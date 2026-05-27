using Lifenote.Application.DTOs.Timer;

namespace Lifenote.Application.Contracts;

public interface ITimerService
{
    Task<IEnumerable<ActiveTimerDto>> GetActiveTimersAsync(Guid userId);
    Task<ActiveTimerDto> StartTimerAsync(Guid userId, StartTimerDto dto);
    Task<ActiveTimerDto> PauseTimerAsync(Guid userId, PauseTimerDto dto);
    Task<ActiveTimerDto> ResetTimerAsync(Guid userId, ResetTimerDto dto);
    Task CompleteTimerAsync(Guid userId, CompleteTimerDto dto);
}
