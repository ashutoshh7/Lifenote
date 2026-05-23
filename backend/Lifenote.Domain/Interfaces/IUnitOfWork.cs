namespace Lifenote.Domain.Interfaces;

/// <summary>
/// Unit of Work contract — defined in Domain so the Application layer
/// depends only on Domain abstractions, not Infrastructure.
/// Wraps all repositories and a single SaveChanges call to keep
/// multiple repository writes in one atomic DB transaction.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    IHabitRepository Habits { get; }
    INoteRepository Notes { get; }
    IUserInfoRepository Users { get; }
    IHabitStreakRepository HabitStreaks { get; }
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
