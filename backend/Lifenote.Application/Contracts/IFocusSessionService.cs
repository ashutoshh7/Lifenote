using Lifenote.Application.DTOs.FocusSession;

namespace Lifenote.Application.Contracts;

public interface IFocusSessionService
{
    Task<IEnumerable<FocusSessionDto>> GetSessionsAsync(int userId);
    Task<FocusSessionDto> CreateSessionAsync(int userId, CreateFocusSessionDto dto);
    Task<object> GetStatsAsync(int userId); // Returns aggregated stats
}
