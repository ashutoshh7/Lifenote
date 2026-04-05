using Lifenote.Application.DTOs.Habit;
using Lifenote.Core.Models;

namespace Lifenote.Application.Contracts;

public interface IHabitStreakService
{
    Task<HabitStreak> GetByHabitIdAsync(int habitId, int userId);
    Task<HabitStreak> CreateInitialStreakAsync(int habitId, int userId);
    Task<HabitStreak> UpdateStreakAfterCheckInAsync(int habitId, int userId, DateTime completionDate);
    Task RecalculateStreakAsync(int habitId, int userId);
    Task<IEnumerable<HabitStreakDto>> GetTopStreaksAsync(int userId, int topN = 5);
    Task<bool> DeleteStreakAsync(int habitId, int userId);
}
