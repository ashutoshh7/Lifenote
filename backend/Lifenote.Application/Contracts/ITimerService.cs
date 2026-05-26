using Lifenote.Application.DTOs.Timer;

namespace Lifenote.Application.Contracts;

public interface ITimerService
{
    Task<IEnumerable<ActiveTimerDto>> GetActiveTimersAsync(int userId);
    Task<ActiveTimerDto> StartTimerAsync(int userId, StartTimerDto dto);
    Task<ActiveTimerDto> PauseTimerAsync(int userId, PauseTimerDto dto);
    Task<ActiveTimerDto> ResetTimerAsync(int userId, ResetTimerDto dto);
    Task CompleteTimerAsync(int userId, CompleteTimerDto dto);
}
