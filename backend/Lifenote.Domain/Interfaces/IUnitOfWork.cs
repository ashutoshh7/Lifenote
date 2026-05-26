namespace Lifenote.Domain.Interfaces;

/// <summary>
/// Unit of Work contract — defined in Domain so the Application layer
/// depends only on Domain abstractions, not Infrastructure.
/// Wraps all repositories and a single SaveChanges call to keep
/// multiple repository writes in one atomic DB transaction.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    IGoalRepository Goals { get; }
    INoteRepository Notes { get; }
    IUserInfoRepository Users { get; }
    IActiveTimerRepository Timers { get; }
    IFocusSessionRepository Sessions { get; }
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
