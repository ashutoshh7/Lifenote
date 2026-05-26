using Lifenote.API.Models.Requests.FocusSession;
using Lifenote.Application.Contracts;
using Lifenote.Application.DTOs.FocusSession;
using Lifenote.API.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Lifenote.API.Controllers;

public class FocusSessionController : ApiControllerBase
{
    private readonly IFocusSessionService _focusSessionService;

    public FocusSessionController(IFocusSessionService focusSessionService, ICurrentUserService currentUserService)
        : base(currentUserService)
    {
        _focusSessionService = focusSessionService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<FocusSessionDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<FocusSessionDto>>>> GetSessions()
    {
        var userId = await GetUserIdAsync();
        var sessions = await _focusSessionService.GetSessionsAsync(userId);
        return Ok(ApiResponse<IEnumerable<FocusSessionDto>>.Success(sessions));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<FocusSessionDto>), StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<FocusSessionDto>>> CreateSession([FromBody] CreateFocusSessionDto dto)
    {
        var userId = await GetUserIdAsync();
        var session = await _focusSessionService.CreateSessionAsync(userId, dto);
        return CreatedAtAction(nameof(GetSessions), new { id = session.Id }, ApiResponse<FocusSessionDto>.Success(session, "Session logged successfully."));
    }

    [HttpGet("stats")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<object>>> GetStats()
    {
        var userId = await GetUserIdAsync();
        var stats = await _focusSessionService.GetStatsAsync(userId);
        return Ok(ApiResponse<object>.Success(stats));
    }
}
