using Lifenote.Domain.Entities;
using Lifenote.Domain.Interfaces;
using Lifenote.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Lifenote.Infrastructure.Repositories;

public class HabitStreakRepository : IHabitStreakRepository
{
    private readonly LifenoteDbContext _db;

    public HabitStreakRepository(LifenoteDbContext db) => _db = db;

    public Task<HabitStreak?> GetByHabitIdAsync(int habitId, int userId) =>
        _db.HabitStreaks.FirstOrDefaultAsync(s => s.HabitId == habitId && s.UserId == userId);

    public async Task CreateAsync(HabitStreak streak) =>
        await _db.HabitStreaks.AddAsync(streak);

    public Task UpdateAsync(HabitStreak streak)
    {
        _db.HabitStreaks.Update(streak);
        return Task.CompletedTask;
    }

    public async Task<bool> DeleteAsync(int habitId, int userId)
    {
        var streak = await _db.HabitStreaks
            .FirstOrDefaultAsync(s => s.HabitId == habitId && s.UserId == userId);
        if (streak == null) return false;
        _db.HabitStreaks.Remove(streak);
        return true;
    }

    public async Task<IEnumerable<HabitStreak>> GetTopStreaksByUserAsync(int userId, int topN) =>
        await _db.HabitStreaks
            .Where(s => s.UserId == userId)
            .Include(s => s.Habit)
            .OrderByDescending(s => s.CurrentStreak)
            .Take(topN)
            .ToListAsync();
}
