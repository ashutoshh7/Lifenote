using Lifenote.Domain.Entities;

namespace Lifenote.Domain.Interfaces;

/// <summary>
/// Repository interface for HabitStreak.
/// HabitStreak belongs to the Habit aggregate — access only via IHabitRepository or this interface.
/// </summary>
public interface IHabitStreakRepository
{
    Task<HabitStreak?> GetByHabitIdAsync(int habitId, int userId);
    Task CreateAsync(HabitStreak streak);
    Task UpdateAsync(HabitStreak streak);
    Task<bool> DeleteAsync(int habitId, int userId);

    /// <summary>
    /// Returns the top N streaks by CurrentStreak for a user — used by GetTopStreaksAsync.
    /// </summary>
    Task<IEnumerable<HabitStreak>> GetTopStreaksByUserAsync(int userId, int topN);
}
