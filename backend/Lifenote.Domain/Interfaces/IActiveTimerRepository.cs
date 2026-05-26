using Lifenote.Domain.Entities;

namespace Lifenote.Domain.Interfaces;

public interface IActiveTimerRepository
{
    Task<ActiveTimer?> GetAsync(int userId, string timerId);
    Task<IEnumerable<ActiveTimer>> GetAllAsync(int userId);
    Task AddAsync(ActiveTimer timer);
    void Update(ActiveTimer timer);
    Task<bool> DeleteAsync(int userId, string timerId);
}
