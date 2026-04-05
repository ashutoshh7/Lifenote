using Lifenote.Application.DTOs.Habit;

namespace Lifenote.Application.Interfaces;

/// <summary>
/// Application service contract for HabitStreak use-cases.
/// Moved from Lifenote.Core.Interfaces — canonical home is now Application layer.
/// </summary>
public interface IHabitStreakService
{
    Task<HabitStreakDto?> GetStreakAsync(int userId, int habitId);
    Task RecalculateStreakAsync(int userId, int habitId);
}
