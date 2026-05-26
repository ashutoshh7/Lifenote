using Lifenote.Domain.Interfaces;
using Lifenote.Infrastructure.Persistence;

namespace Lifenote.Infrastructure.Repositories;

/// <summary>
/// Implements IUnitOfWork defined in Lifenote.Domain.Interfaces.
/// Wraps all repositories and a single SaveChangesAsync call for transactional consistency.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly LifenoteDbContext _db;

    public UnitOfWork(
        LifenoteDbContext db,
        IGoalRepository goals,
        INoteRepository notes,
        IUserInfoRepository users,
        IActiveTimerRepository timers,
        IFocusSessionRepository sessions)
    {
        _db = db;
        Goals = goals;
        Notes = notes;
        Users = users;
        Timers = timers;
        Sessions = sessions;
    }

    public IGoalRepository Goals { get; }
    public INoteRepository Notes { get; }
    public IUserInfoRepository Users { get; }
    public IActiveTimerRepository Timers { get; }
    public IFocusSessionRepository Sessions { get; }

    public Task<int> SaveChangesAsync(CancellationToken ct = default) =>
        _db.SaveChangesAsync(ct);

    public void Dispose() => _db.Dispose();
}
