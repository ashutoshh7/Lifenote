using Lifenote.Application.Contracts;
using Lifenote.Application.DTOs.Timer;
using Lifenote.API.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Lifenote.API.Controllers;

[Route("api/[controller]")]
public class TimerController : ApiControllerBase
{
    private readonly ITimerService _timerService;

    public TimerController(ITimerService timerService, ICurrentUserService currentUserService)
        : base(currentUserService)
    {
        _timerService = timerService;
    }

    /// <summary>Returns all active timers for the authenticated user.</summary>
    [HttpGet("active")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ActiveTimerDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<ActiveTimerDto>>>> GetActiveTimers()
    {
        var userId = await GetUserIdAsync();
        var timers = await _timerService.GetActiveTimersAsync(userId);
        return Ok(ApiResponse<IEnumerable<ActiveTimerDto>>.Success(timers));
    }

    /// <summary>Start a new timer or resume a paused one.</summary>
    [HttpPost("start")]
    [ProducesResponseType(typeof(ApiResponse<ActiveTimerDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<ActiveTimerDto>>> StartTimer([FromBody] StartTimerDto dto)
    {
        var userId = await GetUserIdAsync();
        var timer = await _timerService.StartTimerAsync(userId, dto);
        return Ok(ApiResponse<ActiveTimerDto>.Success(timer));
    }

    /// <summary>Pause a running timer, storing remaining seconds.</summary>
    [HttpPost("pause")]
    [ProducesResponseType(typeof(ApiResponse<ActiveTimerDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<ActiveTimerDto>>> PauseTimer([FromBody] PauseTimerDto dto)
    {
        var userId = await GetUserIdAsync();
        var timer = await _timerService.PauseTimerAsync(userId, dto);
        return Ok(ApiResponse<ActiveTimerDto>.Success(timer));
    }

    /// <summary>Reset a timer back to idle state.</summary>
    [HttpPost("reset")]
    [ProducesResponseType(typeof(ApiResponse<ActiveTimerDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<ActiveTimerDto>>> ResetTimer([FromBody] ResetTimerDto dto)
    {
        var userId = await GetUserIdAsync();
        var timer = await _timerService.ResetTimerAsync(userId, dto);
        return Ok(ApiResponse<ActiveTimerDto>.Success(timer));
    }

    /// <summary>
    /// Mark a timer as completed. Deletes the active timer row and
    /// writes a clean FocusSession history record.
    /// </summary>
    [HttpPost("complete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> CompleteTimer([FromBody] CompleteTimerDto dto)
    {
        var userId = await GetUserIdAsync();
        await _timerService.CompleteTimerAsync(userId, dto);
        return NoContent();
    }
}
