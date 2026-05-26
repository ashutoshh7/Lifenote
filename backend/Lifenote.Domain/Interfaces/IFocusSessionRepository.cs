using Lifenote.Domain.Entities;

namespace Lifenote.Domain.Interfaces;

/// <summary>
/// Minimal repository for FocusSession — used by TimerService to persist
/// completed session history when a timer finishes.
/// </summary>
public interface IFocusSessionRepository
{
    Task<IEnumerable<FocusSession>> GetAllAsync(int userId);
    Task AddAsync(FocusSession session);
}
