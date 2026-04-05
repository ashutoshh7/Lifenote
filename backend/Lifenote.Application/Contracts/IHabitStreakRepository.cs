using Lifenote.Domain.Entities;

namespace Lifenote.Application.Contracts;

public interface IHabitStreakRepository
{
    Task<HabitStreak?> GetByHabitIdAsync(int habitId, int userId);
    Task CreateAsync(HabitStreak streak);
    Task UpdateAsync(HabitStreak streak);
    Task<bool> DeleteAsync(int habitId, int userId);
}
