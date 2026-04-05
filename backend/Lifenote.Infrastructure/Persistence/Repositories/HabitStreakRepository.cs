using Lifenote.Core.Interfaces;
using Lifenote.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Lifenote.Infrastructure.Persistence.Repositories
{
    public class HabitStreakRepository : IHabitStreakRepository
    {
        private readonly LifenoteDbContext _context;

        public HabitStreakRepository(LifenoteDbContext context)
        {
            _context = context;
        }

        public async Task<HabitStreak?> GetByHabitIdAsync(int habitId, int userId)
            => await _context.HabitStreaks
                .Include(s => s.Habit)
                .FirstOrDefaultAsync(s => s.HabitId == habitId && s.UserId == userId);

        public async Task<HabitStreak?> GetByIdAsync(int id, int userId)
            => await _context.HabitStreaks
                .Include(s => s.Habit)
                .FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId);

        public async Task<IEnumerable<HabitStreak>> GetAllByUserIdAsync(int userId)
            => await _context.HabitStreaks
                .Include(s => s.Habit)
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.CurrentStreak)
                .ToListAsync();

        public async Task<HabitStreak> CreateAsync(HabitStreak streak)
        {
            streak.CalculatedAt = DateTime.UtcNow;
            await _context.HabitStreaks.AddAsync(streak);
            return streak;
        }

        public async Task UpdateAsync(HabitStreak streak)
        {
            var existing = await _context.HabitStreaks
                .FirstOrDefaultAsync(s => s.Id == streak.Id && s.UserId == streak.UserId);
            if (existing != null)
            {
                existing.CurrentStreak = streak.CurrentStreak;
                existing.LongestStreak = streak.LongestStreak;
                existing.TotalCompletions = streak.TotalCompletions;
                existing.LastCompletedDate = streak.LastCompletedDate;
                existing.CalculatedAt = DateTime.UtcNow;
            }
        }

        public async Task<bool> DeleteAsync(int id, int userId)
        {
            var streak = await _context.HabitStreaks.FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId);
            if (streak == null) return false;
            _context.HabitStreaks.Remove(streak);
            return true;
        }

        public async Task<bool> ExistsAsync(int habitId, int userId)
            => await _context.HabitStreaks.AnyAsync(s => s.HabitId == habitId && s.UserId == userId);

        public async Task<IEnumerable<HabitStreak>> GetTopStreaksByUserAsync(int userId, int topN = 5)
            => await _context.HabitStreaks
                .Include(s => s.Habit)
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.CurrentStreak)
                .Take(topN)
                .ToListAsync();
    }
}
