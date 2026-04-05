using Lifenote.Application.Contracts;
using Lifenote.Domain.Entities;

namespace Lifenote.Application.Services;

/// <summary>
/// Moved from Lifenote.Data/Services/HabitStreakService.cs.
/// Streak calculation logic is pure business logic — belongs in Application.
/// </summary>
public class HabitStreakService : IHabitStreakService
{
    private readonly IUnitOfWork _unitOfWork;

    public HabitStreakService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public Task<HabitStreak?> GetByHabitIdAsync(int habitId, int userId) =>
        _unitOfWork.HabitStreaks.GetByHabitIdAsync(habitId, userId);

    public async Task CreateInitialStreakAsync(int habitId, int userId)
    {
        var streak = new HabitStreak
        {
            HabitId = habitId,
            UserId = userId,
            CurrentStreak = 0,
            LongestStreak = 0,
            TotalCompletions = 0,
            UpdatedAt = DateTime.UtcNow
        };
        await _unitOfWork.HabitStreaks.CreateAsync(streak);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<HabitStreak> UpdateStreakAfterCheckInAsync(int habitId, int userId, DateTime completedDate)
    {
        var streak = await _unitOfWork.HabitStreaks.GetByHabitIdAsync(habitId, userId);
        if (streak == null)
        {
            await CreateInitialStreakAsync(habitId, userId);
            streak = await _unitOfWork.HabitStreaks.GetByHabitIdAsync(habitId, userId);
        }

        streak!.TotalCompletions++;

        var yesterday = completedDate.AddDays(-1);
        if (streak.LastCompletedDate == yesterday || streak.LastCompletedDate == completedDate)
            streak.CurrentStreak++;
        else
            streak.CurrentStreak = 1;

        if (streak.CurrentStreak > streak.LongestStreak)
            streak.LongestStreak = streak.CurrentStreak;

        streak.LastCompletedDate = completedDate;
        streak.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.HabitStreaks.UpdateAsync(streak);
        await _unitOfWork.SaveChangesAsync();
        return streak;
    }

    public async Task RecalculateStreakAsync(int habitId, int userId)
    {
        var streak = await _unitOfWork.HabitStreaks.GetByHabitIdAsync(habitId, userId);
        if (streak == null) return;

        var logs = await _unitOfWork.Habits.GetLogsAsync(habitId, userId);
        var sortedDates = logs.Select(l => l.CompletedDate).Distinct().OrderByDescending(d => d).ToList();

        streak.TotalCompletions = sortedDates.Count;
        streak.CurrentStreak = 0;
        streak.LastCompletedDate = sortedDates.FirstOrDefault();

        if (sortedDates.Any())
        {
            var current = 1;
            for (var i = 0; i < sortedDates.Count - 1; i++)
            {
                if (sortedDates[i].AddDays(-1) == sortedDates[i + 1])
                    current++;
                else
                    break;
            }
            streak.CurrentStreak = current;
            if (current > streak.LongestStreak) streak.LongestStreak = current;
        }

        streak.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.HabitStreaks.UpdateAsync(streak);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteStreakAsync(int habitId, int userId)
    {
        await _unitOfWork.HabitStreaks.DeleteAsync(habitId, userId);
        await _unitOfWork.SaveChangesAsync();
    }
}
