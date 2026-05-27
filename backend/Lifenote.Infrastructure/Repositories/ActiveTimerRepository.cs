using Lifenote.Domain.Entities;
using Lifenote.Domain.Interfaces;
using Lifenote.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Lifenote.Infrastructure.Repositories;

public class ActiveTimerRepository : IActiveTimerRepository
{
    private readonly LifenoteDbContext _db;

    public ActiveTimerRepository(LifenoteDbContext db) => _db = db;

    public Task<ActiveTimer?> GetAsync(Guid userId, string timerId) =>
        _db.ActiveTimers.FirstOrDefaultAsync(t => t.UserId == userId && t.TimerId == timerId);

    public async Task<IEnumerable<ActiveTimer>> GetAllAsync(Guid userId) =>
        await _db.ActiveTimers
            .Where(t => t.UserId == userId)
            .AsNoTracking()
            .OrderBy(t => t.CreatedAt)
            .ToListAsync();

    public async Task AddAsync(ActiveTimer timer) =>
        await _db.ActiveTimers.AddAsync(timer);

    public void Update(ActiveTimer timer) =>
        _db.ActiveTimers.Update(timer);

    public async Task<bool> DeleteAsync(Guid userId, string timerId)
    {
        var timer = await _db.ActiveTimers
            .FirstOrDefaultAsync(t => t.UserId == userId && t.TimerId == timerId);
        if (timer is null) return false;
        _db.ActiveTimers.Remove(timer);
        return true;
    }
}
