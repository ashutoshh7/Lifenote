using Lifenote.Application.Contracts;
using Lifenote.Application.DTOs.Habit;
using Lifenote.Core.Interfaces;
using Lifenote.Core.Models;

namespace Lifenote.Application.Services
{
    public class HabitStreakService : IHabitStreakService
    {
        private readonly IHabitStreakRepository _streakRepository;
        private readonly IHabitRepository _habitRepository;
        private readonly IUnitOfWork _unitOfWork;

        public HabitStreakService(
            IHabitStreakRepository streakRepository,
            IHabitRepository habitRepository,
            IUnitOfWork unitOfWork)
        {
            _streakRepository = streakRepository;
            _habitRepository = habitRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<HabitStreak> GetByHabitIdAsync(int habitId, int userId)
        {
            var streak = await _streakRepository.GetByHabitIdAsync(habitId, userId);
            if (streak == null) throw new KeyNotFoundException("Streak not found for this habit");
            return streak;
        }

        public async Task<HabitStreak> CreateInitialStreakAsync(int habitId, int userId)
        {
            var existing = await _streakRepository.GetByHabitIdAsync(habitId, userId);
            if (existing != null) throw new InvalidOperationException("Streak already exists for this habit");

            var streak = new HabitStreak
            {
                HabitId = habitId,
                UserId = userId,
                CurrentStreak = 0,
                LongestStreak = 0,
                TotalCompletions = 0,
                CalculatedAt = DateTime.UtcNow
            };

            await _streakRepository.CreateAsync(streak);
            await _unitOfWork.SaveChangesAsync();
            return streak;
        }

        public async Task<HabitStreak> UpdateStreakAfterCheckInAsync(int habitId, int userId, DateTime completionDate)
        {
            var streak = await _streakRepository.GetByHabitIdAsync(habitId, userId);

            if (streak == null)
            {
                streak = new HabitStreak
                {
                    HabitId = habitId,
                    UserId = userId,
                    CurrentStreak = 1,
                    LongestStreak = 1,
                    TotalCompletions = 1,
                    LastCompletedDate = completionDate,
                    CalculatedAt = DateTime.UtcNow
                };
                await _streakRepository.CreateAsync(streak);
            }
            else
            {
                if (streak.LastCompletedDate.HasValue)
                {
                    var daysDiff = (completionDate.Date - streak.LastCompletedDate.Value.Date).Days;
                    if (daysDiff == 1) streak.CurrentStreak++;
                    else if (daysDiff > 1) streak.CurrentStreak = 1;
                }
                else
                {
                    streak.CurrentStreak = 1;
                }

                if (streak.CurrentStreak > streak.LongestStreak)
                    streak.LongestStreak = streak.CurrentStreak;

                streak.TotalCompletions++;
                streak.LastCompletedDate = completionDate;
                streak.CalculatedAt = DateTime.UtcNow;
                await _streakRepository.UpdateAsync(streak);
            }

            await _unitOfWork.SaveChangesAsync();
            return streak;
        }

        public async Task RecalculateStreakAsync(int habitId, int userId)
        {
            var logs = await _habitRepository.GetLogsAsync(habitId, userId);
            var sortedLogs = logs.OrderBy(l => l.CompletedDate).ToList();
            var streak = await _streakRepository.GetByHabitIdAsync(habitId, userId);
            if (streak == null) throw new KeyNotFoundException("Streak not found for this habit");

            if (!sortedLogs.Any())
            {
                streak.CurrentStreak = 0;
                streak.LongestStreak = 0;
                streak.TotalCompletions = 0;
                streak.LastCompletedDate = null;
            }
            else
            {
                var currentStreak = 1;
                var longestStreak = 1;
                for (int i = 1; i < sortedLogs.Count; i++)
                {
                    var daysDiff = (sortedLogs[i].CompletedDate - sortedLogs[i - 1].CompletedDate).Days;
                    if (daysDiff == 1) { currentStreak++; if (currentStreak > longestStreak) longestStreak = currentStreak; }
                    else if (daysDiff > 1) currentStreak = 1;
                }
                streak.CurrentStreak = currentStreak;
                streak.LongestStreak = longestStreak;
                streak.TotalCompletions = sortedLogs.Count;
                streak.LastCompletedDate = sortedLogs.Last().CompletedDate;
            }

            streak.CalculatedAt = DateTime.UtcNow;
            await _streakRepository.UpdateAsync(streak);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<HabitStreakDto>> GetTopStreaksAsync(int userId, int topN = 5)
        {
            var streaks = await _streakRepository.GetTopStreaksByUserAsync(userId, topN);
            return streaks.Select(s => new HabitStreakDto
            {
                Id = s.Id,
                HabitId = s.HabitId,
                HabitName = s.Habit?.Name ?? "Unknown",
                CurrentStreak = s.CurrentStreak,
                LongestStreak = s.LongestStreak,
                TotalCompletions = s.TotalCompletions,
                LastCompletedDate = s.LastCompletedDate
            });
        }

        public async Task<bool> DeleteStreakAsync(int habitId, int userId)
        {
            var streak = await _streakRepository.GetByHabitIdAsync(habitId, userId);
            if (streak == null) return false;
            var result = await _streakRepository.DeleteAsync(streak.Id, userId);
            await _unitOfWork.SaveChangesAsync();
            return result;
        }
    }
}
