using Lifenote.Application.Contracts;
using Lifenote.Application.DTOs.Timer;
using Lifenote.Domain.Entities;
using Lifenote.Domain.Exceptions;
using Lifenote.Domain.Interfaces;

namespace Lifenote.Application.Services;

public class TimerService : ITimerService
{
    private readonly IUnitOfWork _uow;

    public TimerService(IUnitOfWork uow) => _uow = uow;

    // ─── Query ───────────────────────────────────────────────────────────────

    public async Task<IEnumerable<ActiveTimerDto>> GetActiveTimersAsync(Guid userId)
    {
        var timers = await _uow.Timers.GetAllAsync(userId);
        return timers.Select(MapToDto);
    }

    // ─── Start / Resume ──────────────────────────────────────────────────────

    public async Task<ActiveTimerDto> StartTimerAsync(Guid userId, StartTimerDto dto)
    {
        var existing = await _uow.Timers.GetAsync(userId, dto.TimerId);

        if (existing is null)
        {
            // Brand-new timer — insert
            var timer = new ActiveTimer
            {
                UserId               = userId,
                TimerId              = dto.TimerId,
                Label                = dto.Label,
                SessionType          = dto.SessionType,
                Status               = "running",
                TotalDurationSeconds = dto.TotalDurationSeconds,
                StartedAt            = DateTimeOffset.UtcNow,
                RemainingSeconds     = null
            };
            await _uow.Timers.AddAsync(timer);
            await _uow.SaveChangesAsync();
            return MapToDto(timer);
        }

        // Resume a paused timer — recalculate StartedAt so that
        // (StartedAt + TotalDurationSeconds) == the correct end time
        // given the remaining seconds the client reported when pausing.
        var remainingToUse = existing.RemainingSeconds ?? existing.TotalDurationSeconds;
        existing.Status           = "running";
        existing.StartedAt        = DateTimeOffset.UtcNow.AddSeconds(-existing.TotalDurationSeconds + remainingToUse);
        existing.RemainingSeconds = null;
        existing.Label            = dto.Label; // allow label updates on resume
        _uow.Timers.Update(existing);
        await _uow.SaveChangesAsync();
        return MapToDto(existing);
    }

    // ─── Pause ───────────────────────────────────────────────────────────────

    public async Task<ActiveTimerDto> PauseTimerAsync(Guid userId, PauseTimerDto dto)
    {
        var timer = await _uow.Timers.GetAsync(userId, dto.TimerId)
            ?? throw new NotFoundException($"Active timer '{dto.TimerId}' not found.");

        timer.Status           = "paused";
        timer.StartedAt        = null;
        timer.RemainingSeconds = Math.Max(0, dto.RemainingSeconds);
        _uow.Timers.Update(timer);
        await _uow.SaveChangesAsync();
        return MapToDto(timer);
    }

    // ─── Reset ───────────────────────────────────────────────────────────────

    public async Task<ActiveTimerDto> ResetTimerAsync(Guid userId, ResetTimerDto dto)
    {
        var timer = await _uow.Timers.GetAsync(userId, dto.TimerId)
            ?? throw new NotFoundException($"Active timer '{dto.TimerId}' not found.");

        timer.Status           = "idle";
        timer.StartedAt        = null;
        timer.RemainingSeconds = null;
        _uow.Timers.Update(timer);
        await _uow.SaveChangesAsync();
        return MapToDto(timer);
    }

    // ─── Complete ─────────────────────────────────────────────────────────────

    public async Task CompleteTimerAsync(Guid userId, CompleteTimerDto dto)
    {
        var timer = await _uow.Timers.GetAsync(userId, dto.TimerId)
            ?? throw new NotFoundException($"Active timer '{dto.TimerId}' not found.");

        // Write a clean, completed FocusSession history record
        var endTime   = DateTimeOffset.UtcNow;
        var startTime = timer.StartedAt
            ?? endTime.AddSeconds(-dto.ActualDurationSeconds);

        var session = new FocusSession
        {
            UserId         = userId,
            SessionType    = timer.SessionType,
            Duration       = timer.TotalDurationSeconds,
            ActualDuration = dto.ActualDurationSeconds,
            StartTime      = startTime,
            EndTime        = endTime,
            IsCompleted    = true,
            Notes          = dto.Notes
        };

        await _uow.Sessions.AddAsync(session);

        // Remove the scratch timer row
        await _uow.Timers.DeleteAsync(userId, dto.TimerId);

        await _uow.SaveChangesAsync();
    }

    // ─── Mapping ─────────────────────────────────────────────────────────────

    private static ActiveTimerDto MapToDto(ActiveTimer t) => new()
    {
        TimerId              = t.TimerId,
        Label                = t.Label,
        SessionType          = t.SessionType,
        Status               = t.Status,
        TotalDurationSeconds = t.TotalDurationSeconds,
        StartedAt            = t.StartedAt,
        RemainingSeconds     = t.RemainingSeconds
    };
}
