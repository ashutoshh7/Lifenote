using Lifenote.Domain.Entities;
using Lifenote.Domain.Interfaces;
using Lifenote.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Lifenote.Infrastructure.Repositories;

public class GoalRepository : IGoalRepository
{
    private readonly LifenoteDbContext _db;

    public GoalRepository(LifenoteDbContext db) => _db = db;

    public Task<Goal?> GetByIdAsync(int id) =>
        _db.Goals
            .Include(g => g.Milestones)
            .FirstOrDefaultAsync(g => g.Id == id);

    public async Task<IEnumerable<Goal>> GetByUserIdAsync(int userId) =>
        await _db.Goals
            .Include(g => g.Milestones)
            .Where(g => g.UserId == userId)
            .ToListAsync();

    public async Task AddAsync(Goal goal) =>
        await _db.Goals.AddAsync(goal);

    public void Update(Goal goal) =>
        _db.Goals.Update(goal);

    public void Remove(Goal goal) =>
        _db.Goals.Remove(goal);
}
