using Lifenote.Application.DTOs.FocusSession;

namespace Lifenote.Application.Contracts;

public interface IFocusSessionService
{
    Task<IEnumerable<FocusSessionDto>> GetSessionsAsync(string userId);
    Task<FocusSessionDto> CreateSessionAsync(string userId, CreateFocusSessionDto dto);
    Task<object> GetStatsAsync(string userId); // Returns aggregated stats
}
