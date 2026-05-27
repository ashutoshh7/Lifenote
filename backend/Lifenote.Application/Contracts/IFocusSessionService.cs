using Lifenote.Application.DTOs.FocusSession;

namespace Lifenote.Application.Contracts;

public interface IFocusSessionService
{
    Task<IEnumerable<FocusSessionDto>> GetSessionsAsync(Guid userId);
    Task<FocusSessionDto> CreateSessionAsync(Guid userId, CreateFocusSessionDto dto);
    Task<object> GetStatsAsync(Guid userId); // Returns aggregated stats
}
