using Lifenote.Application.Contracts;
using Lifenote.Domain.Entities;
using Lifenote.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Lifenote.Infrastructure.Repositories;

/// <summary>
/// Moved from Lifenote.Data/Repositories/HabitRepository.cs.
/// Now implements the IHabitRepository interface defined in Application.
/// All EF Core usage stays here — Application layer never sees DbContext.
/// </summary>
public class HabitRepository : IHabitRepository
{
    private readonly LifenoteDbContext _db;

    public HabitRepository(LifenoteDbContext db) => _db = db;

    public Task<Habit?> GetByIdAsync(int habitId, int userId) =>
        _db.Habits.FirstOrDefaultAsync(h => h.Id == habitId && h.UserId == userId);

    public async Task<IEnumerable<Habit>> GetAllAsync(int userId, bool includeInactive = false)
    {
        var query = _db.Habits.Where(h => h.UserId == userId);
        if (!includeInactive) query = query.Where(h => h.IsActive);
        return await query.AsNoTracking().ToListAsync();
    }

    public async Task<IEnumerable<Habit>> GetHabitsWithTodayStatusAsync(int userId, DateTime today) =>
        await _db.Habits
            .Where(h => h.UserId == userId)
            .Include(h => h.Logs.Where(l => l.CompletedDate == today))
            .AsNoTracking()
            .ToListAsync();

    public async Task CreateAsync(Habit habit) => await _db.Habits.AddAsync(habit);

    public Task UpdateAsync(Habit habit)
    {
        _db.Habits.Update(habit);
        return Task.CompletedTask;
    }

    public async Task<bool> DeleteAsync(int habitId, int userId)
    {
        var habit = await _db.Habits.FirstOrDefaultAsync(h => h.Id == habitId && h.UserId == userId);
        if (habit == null) return false;
        _db.Habits.Remove(habit);
        return true;
    }

    public Task<bool> ExistsAsync(int habitId, int userId) =>
        _db.Habits.AnyAsync(h => h.Id == habitId && h.UserId == userId);

    public Task<int> GetTodayLogCountAsync(int habitId, int userId, DateTime today) =>
        _db.HabitLogs.CountAsync(l => l.HabitId == habitId && l.UserId == userId && l.CompletedDate == today);

    public Task<HabitLog?> GetTodayLogAsync(int habitId, int userId, DateTime today) =>
        _db.HabitLogs.FirstOrDefaultAsync(l => l.HabitId == habitId && l.UserId == userId && l.CompletedDate == today);

    public async Task AddLogAsync(HabitLog log) => await _db.HabitLogs.AddAsync(log);

    public async Task RemoveLogAsync(int logId, int userId)
    {
        var log = await _db.HabitLogs.FirstOrDefaultAsync(l => l.Id == logId && l.UserId == userId);
        if (log != null) _db.HabitLogs.Remove(log);
    }

    public async Task<IEnumerable<HabitLog>> GetLogsAsync(int habitId, int userId, DateTime? startDate = null, DateTime? endDate = null)
    {
        var query = _db.HabitLogs.Where(l => l.HabitId == habitId && l.UserId == userId);
        if (startDate.HasValue) query = query.Where(l => l.CompletedDate >= startDate.Value);
        if (endDate.HasValue) query = query.Where(l => l.CompletedDate <= endDate.Value);
        return await query.AsNoTracking().OrderBy(l => l.CompletedDate).ToListAsync();
    }
}
