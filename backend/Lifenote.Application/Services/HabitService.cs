using Lifenote.Application.Contracts;
using Lifenote.Application.DTOs.Habit;
using Lifenote.Core.Interfaces;
using Lifenote.Core.Models;

namespace Lifenote.Application.Services
{
    public class HabitService : IHabitService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHabitStreakService _habitStreakService;

        public HabitService(IUnitOfWork unitOfWork, IHabitStreakService habitStreakService)
        {
            _unitOfWork = unitOfWork;
            _habitStreakService = habitStreakService;
        }

        public async Task<HabitDto> CreateHabitAsync(int userId, CreateHabitDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Habit name is required");

            if (dto.FrequencyType == "Custom" && string.IsNullOrWhiteSpace(dto.FrequencyValue))
                throw new ArgumentException("Custom frequency requires days to be specified");

            var habit = new Habit
            {
                UserId = userId,
                Name = dto.Name.Trim(),
                Description = dto.Description?.Trim(),
                Color = dto.Color,
                IconName = dto.IconName,
                FrequencyType = dto.FrequencyType,
                FrequencyValue = dto.FrequencyValue,
                TargetCount = dto.TargetCount,
                StartDate = dto.StartDate ?? DateTime.UtcNow.Date,
                EndDate = dto.EndDate,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Habits.CreateAsync(habit);
            await _unitOfWork.SaveChangesAsync();

            await _habitStreakService.CreateInitialStreakAsync(habit.Id, userId);

            return await MapToHabitDto(habit, userId);
        }

        public async Task<HabitDto> GetHabitByIdAsync(int userId, int habitId)
        {
            var habit = await _unitOfWork.Habits.GetByIdAsync(habitId, userId);
            if (habit == null)
                throw new KeyNotFoundException("Habit not found");
            return await MapToHabitDto(habit, userId);
        }

        public async Task<IEnumerable<HabitDto>> GetUserHabitsAsync(int userId, bool includeInactive = false)
        {
            var habits = await _unitOfWork.Habits.GetHabitsWithTodayStatusAsync(userId, DateTime.UtcNow.Date);
            if (!includeInactive)
                habits = habits.Where(h => h.IsActive);

            var habitDtos = new List<HabitDto>();
            foreach (var habit in habits)
                habitDtos.Add(await MapToHabitDto(habit, userId));
            return habitDtos;
        }

        public async Task<HabitDto> UpdateHabitAsync(int userId, int habitId, UpdateHabitDto dto)
        {
            var habit = await _unitOfWork.Habits.GetByIdAsync(habitId, userId);
            if (habit == null)
                throw new KeyNotFoundException("Habit not found");

            if (!string.IsNullOrWhiteSpace(dto.Name)) habit.Name = dto.Name.Trim();
            if (dto.Description != null) habit.Description = dto.Description.Trim();
            if (!string.IsNullOrWhiteSpace(dto.Color)) habit.Color = dto.Color;
            if (!string.IsNullOrWhiteSpace(dto.IconName)) habit.IconName = dto.IconName;
            if (!string.IsNullOrWhiteSpace(dto.FrequencyType))
            {
                habit.FrequencyType = dto.FrequencyType;
                habit.FrequencyValue = dto.FrequencyValue;
            }
            if (dto.TargetCount.HasValue) habit.TargetCount = dto.TargetCount.Value;
            if (dto.IsActive.HasValue) habit.IsActive = dto.IsActive.Value;
            if (dto.EndDate.HasValue) habit.EndDate = dto.EndDate.Value;
            habit.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Habits.UpdateAsync(habit);
            await _unitOfWork.SaveChangesAsync();

            return await MapToHabitDto(habit, userId);
        }

        public async Task<bool> DeleteHabitAsync(int userId, int habitId)
        {
            var exists = await _unitOfWork.Habits.ExistsAsync(habitId, userId);
            if (!exists) return false;

            await _habitStreakService.DeleteStreakAsync(habitId, userId);
            var result = await _unitOfWork.Habits.DeleteAsync(habitId, userId);
            await _unitOfWork.SaveChangesAsync();
            return result;
        }

        public async Task<bool> ToggleHabitStatusAsync(int userId, int habitId)
        {
            var habit = await _unitOfWork.Habits.GetByIdAsync(habitId, userId);
            if (habit == null) return false;

            habit.IsActive = !habit.IsActive;
            habit.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.Habits.UpdateAsync(habit);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<HabitLogDto> CheckInHabitAsync(int userId, CheckInDto dto)
        {
            var habit = await _unitOfWork.Habits.GetByIdAsync(dto.HabitId, userId);
            if (habit == null) throw new KeyNotFoundException("Habit not found");
            if (!habit.IsActive) throw new InvalidOperationException("Cannot check in to an inactive habit");

            var today = DateTime.UtcNow.Date;
            var todayCount = await _unitOfWork.Habits.GetTodayLogCountAsync(dto.HabitId, userId, today);
            if (todayCount >= habit.TargetCount)
                throw new InvalidOperationException($"You've already completed this habit {habit.TargetCount} time(s) today");

            var log = new HabitLog
            {
                HabitId = dto.HabitId,
                UserId = userId,
                CompletedAt = DateTime.UtcNow,
                CompletedDate = today,
                Notes = dto.Notes,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Habits.AddLogAsync(log);
            await _unitOfWork.SaveChangesAsync();

            var streak = await _habitStreakService.UpdateStreakAfterCheckInAsync(dto.HabitId, userId, today);

            return new HabitLogDto
            {
                Id = log.Id,
                HabitId = log.HabitId,
                HabitName = habit.Name,
                CompletedAt = log.CompletedAt,
                CompletedDate = log.CompletedDate,
                Notes = log.Notes,
                CurrentStreak = streak.CurrentStreak
            };
        }

        public async Task<bool> UndoCheckInAsync(int userId, int habitId)
        {
            var today = DateTime.UtcNow.Date;
            var log = await _unitOfWork.Habits.GetTodayLogAsync(habitId, userId, today);
            if (log == null) return false;

            await _unitOfWork.Habits.RemoveLogAsync(log.Id, userId);
            await _unitOfWork.SaveChangesAsync();
            await _habitStreakService.RecalculateStreakAsync(habitId, userId);
            return true;
        }

        public async Task<IEnumerable<HabitLogDto>> GetHabitHistoryAsync(int userId, int habitId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var habit = await _unitOfWork.Habits.GetByIdAsync(habitId, userId);
            if (habit == null) throw new KeyNotFoundException("Habit not found");

            var logs = await _unitOfWork.Habits.GetLogsAsync(habitId, userId, startDate, endDate);
            return logs.Select(l => new HabitLogDto
            {
                Id = l.Id,
                HabitId = l.HabitId,
                HabitName = habit.Name,
                CompletedAt = l.CompletedAt,
                CompletedDate = l.CompletedDate,
                Notes = l.Notes
            });
        }

        public async Task<HabitStatisticsDto> GetHabitStatisticsAsync(int userId, int habitId)
        {
            var habit = await _unitOfWork.Habits.GetByIdAsync(habitId, userId);
            if (habit == null) throw new KeyNotFoundException("Habit not found");

            var streak = await _habitStreakService.GetByHabitIdAsync(habitId, userId);
            var thirtyDaysAgo = DateTime.UtcNow.Date.AddDays(-30);
            var logs = await _unitOfWork.Habits.GetLogsAsync(habitId, userId, thirtyDaysAgo);
            var logsList = logs.ToList();

            var completionRate = logsList.Count > 0 ? (double)logsList.Count / 30 * 100 : 0;

            var dayGroups = logsList
                .GroupBy(l => l.CompletedDate.DayOfWeek)
                .Select(g => new { Day = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ToList();

            var sevenDaysAgo = DateTime.UtcNow.Date.AddDays(-7);
            var last7Days = Enumerable.Range(0, 7)
                .Select(i => sevenDaysAgo.AddDays(i))
                .Select(date => new DailyActivityDto
                {
                    Date = date,
                    Completed = logsList.Any(l => l.CompletedDate == date),
                    Notes = logsList.FirstOrDefault(l => l.CompletedDate == date)?.Notes
                })
                .ToList();

            return new HabitStatisticsDto
            {
                HabitId = habitId,
                HabitName = habit.Name,
                CurrentStreak = streak?.CurrentStreak ?? 0,
                LongestStreak = streak?.LongestStreak ?? 0,
                TotalCompletions = streak?.TotalCompletions ?? 0,
                CompletionRate = Math.Round(completionRate, 2),
                BestDayOfWeek = dayGroups.FirstOrDefault()?.Day.ToString(),
                WorstDayOfWeek = dayGroups.LastOrDefault()?.Day.ToString(),
                Last7Days = last7Days
            };
        }

        public async Task<WeeklyCalendarDto> GetWeeklyCalendarAsync(int userId, DateTime weekStart)
        {
            var weekEnd = weekStart.AddDays(7);
            var habits = await _unitOfWork.Habits.GetAllAsync(userId, includeInactive: false);
            var habitsList = habits.ToList();
            var habitWeeks = new List<HabitWeekDto>();

            foreach (var habit in habitsList)
            {
                var logs = await _unitOfWork.Habits.GetLogsAsync(habit.Id, userId, weekStart, weekEnd);
                var completedDates = logs.Select(l => l.CompletedDate).ToList();
                var expectedCount = CalculateExpectedCount(habit.FrequencyType, habit.FrequencyValue, weekStart, weekEnd);
                var completionRate = expectedCount > 0 ? (double)completedDates.Count / expectedCount * 100 : 0;

                habitWeeks.Add(new HabitWeekDto
                {
                    HabitId = habit.Id,
                    Name = habit.Name,
                    Color = habit.Color,
                    IconName = habit.IconName,
                    FrequencyType = habit.FrequencyType,
                    CompletedDates = completedDates,
                    CompletedCount = completedDates.Count,
                    ExpectedCount = expectedCount,
                    CompletionRate = Math.Round(completionRate, 2)
                });
            }

            return new WeeklyCalendarDto
            {
                WeekStart = weekStart,
                WeekEnd = weekEnd,
                TotalHabits = habitsList.Count,
                OverallCompletionRate = Math.Round(habitWeeks.Any() ? habitWeeks.Average(h => h.CompletionRate) : 0, 2),
                Habits = habitWeeks
            };
        }

        private async Task<HabitDto> MapToHabitDto(Habit habit, int userId)
        {
            var streak = await _habitStreakService.GetByHabitIdAsync(habit.Id, userId);
            var today = DateTime.UtcNow.Date;
            var todayCount = await _unitOfWork.Habits.GetTodayLogCountAsync(habit.Id, userId, today);

            return new HabitDto
            {
                Id = habit.Id,
                Name = habit.Name,
                Description = habit.Description,
                Color = habit.Color,
                IconName = habit.IconName,
                FrequencyType = habit.FrequencyType,
                FrequencyValue = habit.FrequencyValue,
                TargetCount = habit.TargetCount,
                IsActive = habit.IsActive,
                StartDate = habit.StartDate,
                EndDate = habit.EndDate,
                CurrentStreak = streak?.CurrentStreak ?? 0,
                LongestStreak = streak?.LongestStreak ?? 0,
                TotalCompletions = streak?.TotalCompletions ?? 0,
                CompletedToday = todayCount >= habit.TargetCount,
                CompletedCountToday = todayCount,
                CreatedAt = habit.CreatedAt,
                UpdatedAt = habit.UpdatedAt
            };
        }

        private int CalculateExpectedCount(string frequencyType, string? frequencyValue, DateTime startDate, DateTime endDate)
        {
            var days = (endDate - startDate).Days;
            return frequencyType switch
            {
                "Daily" => days,
                "Weekly" => days >= 7 ? 1 : 0,
                "Custom" => CalculateCustomFrequencyCount(frequencyValue, startDate, endDate),
                _ => days
            };
        }

        private int CalculateCustomFrequencyCount(string? frequencyValue, DateTime startDate, DateTime endDate)
        {
            if (string.IsNullOrEmpty(frequencyValue)) return 0;
            var days = System.Text.Json.JsonSerializer.Deserialize<List<string>>(frequencyValue);
            if (days == null) return 0;
            var count = 0;
            for (var date = startDate; date < endDate; date = date.AddDays(1))
                if (days.Contains(date.DayOfWeek.ToString())) count++;
            return count;
        }
    }
}
