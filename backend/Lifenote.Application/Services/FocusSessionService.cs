using Lifenote.Application.Contracts;
using Lifenote.Application.DTOs.FocusSession;
using Lifenote.Domain.Entities;
using Lifenote.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lifenote.Application.Services
{
    public class FocusSessionService : IFocusSessionService
    {
        private readonly IUnitOfWork _uow;

        public FocusSessionService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<IEnumerable<FocusSessionDto>> GetSessionsAsync(Guid userId)
        {
            var sessions = await _uow.Sessions.GetAllAsync(userId);
            return sessions.Select(MapToDto);
        }

        public async Task<FocusSessionDto> CreateSessionAsync(Guid userId, CreateFocusSessionDto dto)
        {
            var session = new FocusSession
            {
                UserId = userId,
                SessionType = dto.SessionType,
                Duration = dto.DurationMinutes * 60,
                ActualDuration = dto.DurationMinutes * 60,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                IsCompleted = true,
                Notes = dto.Notes
            };

            await _uow.Sessions.AddAsync(session);
            await _uow.SaveChangesAsync();

            return MapToDto(session);
        }

        public async Task<object> GetStatsAsync(Guid userId)
        {
            var sessions = await _uow.Sessions.GetAllAsync(userId);
            var todayUtc = DateTime.UtcNow.Date;

            // Filter sessions completed today (based on local user date - here using UTC as base)
            var todaySessions = sessions.Where(s => s.EndTime.HasValue && s.EndTime.Value.UtcDateTime.Date == todayUtc && s.IsCompleted);
            var todayFocusSeconds = todaySessions.Sum(s => s.ActualDuration ?? s.Duration);
            var todayFocusHours = Math.Round((double)todayFocusSeconds / 3600.0, 1);

            // Calculate daily streak
            var sessionDates = sessions
                .Where(s => s.EndTime.HasValue && s.IsCompleted)
                .Select(s => s.EndTime.Value.UtcDateTime.Date)
                .Distinct()
                .OrderByDescending(d => d)
                .ToList();

            int streak = 0;
            var checkDate = todayUtc;

            if (sessionDates.Contains(checkDate))
            {
                while (sessionDates.Contains(checkDate))
                {
                    streak++;
                    checkDate = checkDate.AddDays(-1);
                }
            }
            else if (sessionDates.Contains(checkDate.AddDays(-1)))
            {
                checkDate = checkDate.AddDays(-1);
                while (sessionDates.Contains(checkDate))
                {
                    streak++;
                    checkDate = checkDate.AddDays(-1);
                }
            }

            return new
            {
                TodayFocusHours = todayFocusHours,
                CurrentStreak = streak,
                TotalSessionsCompleted = sessions.Count(s => s.IsCompleted)
            };
        }

        private static FocusSessionDto MapToDto(FocusSession s) => new()
        {
            Id = s.Id,
            UserId = s.UserId.ToString(),
            StartTime = s.StartTime?.UtcDateTime ?? DateTime.UtcNow,
            EndTime = s.EndTime?.UtcDateTime ?? DateTime.UtcNow,
            DurationMinutes = s.Duration / 60,
            SessionType = s.SessionType,
            Label = s.SessionType,
            Notes = s.Notes
        };
    }
}
