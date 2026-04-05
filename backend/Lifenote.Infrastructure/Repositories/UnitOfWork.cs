using Lifenote.Application.Contracts;
using Lifenote.Infrastructure.Persistence;

namespace Lifenote.Infrastructure.Repositories;

/// <summary>
/// Moved from Lifenote.Data/Repositories/UnitOfWork.cs.
/// Implements IUnitOfWork defined in Application. Wraps all repositories
/// and a single SaveChangesAsync call for transactional consistency.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly LifenoteDbContext _db;

    public UnitOfWork(
        LifenoteDbContext db,
        IHabitRepository habits,
        INoteRepository notes,
        IUserInfoRepository users,
        IHabitStreakRepository habitStreaks)
    {
        _db = db;
        Habits = habits;
        Notes = notes;
        Users = users;
        HabitStreaks = habitStreaks;
    }

    public IHabitRepository Habits { get; }
    public INoteRepository Notes { get; }
    public IUserInfoRepository Users { get; }
    public IHabitStreakRepository HabitStreaks { get; }

    public Task<int> SaveChangesAsync(CancellationToken ct = default) =>
        _db.SaveChangesAsync(ct);

    public void Dispose() => _db.Dispose();
}
