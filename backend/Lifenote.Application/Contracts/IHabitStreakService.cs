using Lifenote.Domain.Entities;

namespace Lifenote.Application.Contracts;

public interface IHabitStreakService
{
    Task<HabitStreak?> GetByHabitIdAsync(int habitId, int userId);
    Task CreateInitialStreakAsync(int habitId, int userId);
    Task<HabitStreak> UpdateStreakAfterCheckInAsync(int habitId, int userId, DateTime completedDate);
    Task RecalculateStreakAsync(int habitId, int userId);
    Task DeleteStreakAsync(int habitId, int userId);
}
