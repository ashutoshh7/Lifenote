namespace Lifenote.Application.Contracts;

/// <summary>
/// Unit of Work wraps all repositories and a single SaveChanges call.
/// Keeps multiple repository writes in one DB transaction.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    IHabitRepository Habits { get; }
    INoteRepository Notes { get; }
    IUserInfoRepository Users { get; }
    IHabitStreakRepository HabitStreaks { get; }
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
