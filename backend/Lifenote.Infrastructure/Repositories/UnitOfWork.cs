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
        IUserInfoRepository users)
    {
        _db = db;
        Goals = goals;
        Notes = notes;
        Users = users;
    }

    public IGoalRepository Goals { get; }
    public INoteRepository Notes { get; }
    public IUserInfoRepository Users { get; }

    public Task<int> SaveChangesAsync(CancellationToken ct = default) =>
        _db.SaveChangesAsync(ct);

    public void Dispose() => _db.Dispose();
}
