using Lifenote.Application.Contracts;
using Lifenote.Domain.Entities;
using Lifenote.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Lifenote.Infrastructure.Repositories;

/// <summary>
/// Moved from Lifenote.Data/Repositories/HabitStreakRepository.cs.
/// </summary>
public class HabitStreakRepository : IHabitStreakRepository
{
    private readonly LifenoteDbContext _db;

    public HabitStreakRepository(LifenoteDbContext db) => _db = db;

    public Task<HabitStreak?> GetByHabitIdAsync(int habitId, int userId) =>
        _db.HabitStreaks.FirstOrDefaultAsync(s => s.HabitId == habitId && s.UserId == userId);

    public async Task CreateAsync(HabitStreak streak) => await _db.HabitStreaks.AddAsync(streak);

    public Task UpdateAsync(HabitStreak streak)
    {
        _db.HabitStreaks.Update(streak);
        return Task.CompletedTask;
    }

    public async Task<bool> DeleteAsync(int habitId, int userId)
    {
        var streak = await _db.HabitStreaks.FirstOrDefaultAsync(s => s.HabitId == habitId && s.UserId == userId);
        if (streak == null) return false;
        _db.HabitStreaks.Remove(streak);
        return true;
    }
}
