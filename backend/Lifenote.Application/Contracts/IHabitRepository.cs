using Lifenote.Domain.Entities;

namespace Lifenote.Application.Contracts;

/// <summary>
/// Interface defined in Application layer.
/// Infrastructure layer provides the EF Core implementation.
/// </summary>
public interface IHabitRepository
{
    Task<Habit?> GetByIdAsync(int habitId, int userId);
    Task<IEnumerable<Habit>> GetAllAsync(int userId, bool includeInactive = false);
    Task<IEnumerable<Habit>> GetHabitsWithTodayStatusAsync(int userId, DateTime today);
    Task CreateAsync(Habit habit);
    Task UpdateAsync(Habit habit);
    Task<bool> DeleteAsync(int habitId, int userId);
    Task<bool> ExistsAsync(int habitId, int userId);
    Task<int> GetTodayLogCountAsync(int habitId, int userId, DateTime today);
    Task<HabitLog?> GetTodayLogAsync(int habitId, int userId, DateTime today);
    Task AddLogAsync(HabitLog log);
    Task RemoveLogAsync(int logId, int userId);
    Task<IEnumerable<HabitLog>> GetLogsAsync(int habitId, int userId, DateTime? startDate = null, DateTime? endDate = null);
}
