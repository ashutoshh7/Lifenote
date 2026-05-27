using Lifenote.Domain.Entities;

namespace Lifenote.Domain.Interfaces;

/// <summary>
/// Repository interface for Goal aggregate.
/// </summary>
public interface IGoalRepository
{
    Task<Goal?> GetByIdAsync(Guid id);
    Task<IEnumerable<Goal>> GetByUserIdAsync(Guid userId);
    Task AddAsync(Goal goal);
    void Update(Goal goal);
    void Remove(Goal goal);
}
