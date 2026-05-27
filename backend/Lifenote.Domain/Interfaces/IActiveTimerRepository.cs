using Lifenote.Domain.Entities;

namespace Lifenote.Domain.Interfaces;

public interface IActiveTimerRepository
{
    Task<ActiveTimer?> GetAsync(Guid userId, string timerId);
    Task<IEnumerable<ActiveTimer>> GetAllAsync(Guid userId);
    Task AddAsync(ActiveTimer timer);
    void Update(ActiveTimer timer);
    Task<bool> DeleteAsync(Guid userId, string timerId);
}
