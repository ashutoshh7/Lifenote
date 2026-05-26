using Lifenote.Domain.Entities;
using Lifenote.Domain.Interfaces;
using Lifenote.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Lifenote.Infrastructure.Repositories;

public class FocusSessionRepository : IFocusSessionRepository
{
    private readonly LifenoteDbContext _db;

    public FocusSessionRepository(LifenoteDbContext db) => _db = db;

    public async Task<IEnumerable<FocusSession>> GetAllAsync(int userId) =>
        await _db.FocusSessions
            .Where(s => s.UserId == userId)
            .AsNoTracking()
            .OrderByDescending(s => s.StartTime)
            .ToListAsync();

    public async Task AddAsync(FocusSession session) =>
        await _db.FocusSessions.AddAsync(session);
}
