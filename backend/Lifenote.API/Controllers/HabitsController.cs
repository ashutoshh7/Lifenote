using Lifenote.API.Mappings;
using Lifenote.API.Models.Requests.Habit;
using Lifenote.API.Models.Responses;
using Lifenote.Application.Contracts;
using Lifenote.Application.DTOs.Habit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lifenote.API.Controllers;

/// <summary>
/// Habits controller — Phase 2 Step 3.
/// - Accepts API request models (CreateHabitRequest, UpdateHabitRequest, CheckInRequest).
/// - Returns ApiResponse<T> envelope; never exposes Application DTOs directly.
/// - Maps request → DTO via RequestMappingExtensions.ToDto().
/// - No try/catch — ExceptionHandlingMiddleware handles all exceptions globally.
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class HabitsController : ControllerBase
{
    private readonly IHabitService _habitService;
    private readonly ICurrentUserService _currentUserService;

    public HabitsController(IHabitService habitService, ICurrentUserService currentUserService)
    {
        _habitService = habitService;
        _currentUserService = currentUserService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<HabitDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<HabitDto>>>> GetHabits(
        [FromQuery] bool includeInactive = false)
    {
        var userId = await _currentUserService.GetCurrentUserIdAsync();
        var habits = await _habitService.GetUserHabitsAsync(userId, includeInactive);
        return Ok(ApiResponse<IEnumerable<HabitDto>>.Success(habits));
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<HabitDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<HabitDto>>> GetHabit(int id)
    {
        var userId = await _currentUserService.GetCurrentUserIdAsync();
        var habit = await _habitService.GetHabitByIdAsync(userId, id);
        return Ok(ApiResponse<HabitDto>.Success(habit));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<HabitDto>), StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<HabitDto>>> CreateHabit(
        [FromBody] CreateHabitRequest request)
    {
        var userId = await _currentUserService.GetCurrentUserIdAsync();
        var habit = await _habitService.CreateHabitAsync(userId, request.ToDto());
        return CreatedAtAction(
            nameof(GetHabit),
            new { id = habit.Id },
            ApiResponse<HabitDto>.Success(habit, "Habit created successfully."));
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<HabitDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<HabitDto>>> UpdateHabit(
        int id, [FromBody] UpdateHabitRequest request)
    {
        var userId = await _currentUserService.GetCurrentUserIdAsync();
        var habit = await _habitService.UpdateHabitAsync(userId, id, request.ToDto());
        return Ok(ApiResponse<HabitDto>.Success(habit));
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object>>> DeleteHabit(int id)
    {
        var userId = await _currentUserService.GetCurrentUserIdAsync();
        var result = await _habitService.DeleteHabitAsync(userId, id);
        if (!result)
            return NotFound(ApiResponse<object>.Fail("Habit not found."));
        return Ok(ApiResponse<object>.Success(new { }, "Habit deleted successfully."));
    }

    [HttpPatch("{id}/toggle")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object>>> ToggleHabitStatus(int id)
    {
        var userId = await _currentUserService.GetCurrentUserIdAsync();
        var result = await _habitService.ToggleHabitStatusAsync(userId, id);
        if (!result)
            return NotFound(ApiResponse<object>.Fail("Habit not found."));
        return Ok(ApiResponse<object>.Success(new { }, "Habit status toggled successfully."));
    }

    [HttpPost("checkin")]
    [ProducesResponseType(typeof(ApiResponse<HabitLogDto>), StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<HabitLogDto>>> CheckIn(
        [FromBody] CheckInRequest request)
    {
        var userId = await _currentUserService.GetCurrentUserIdAsync();
        var log = await _habitService.CheckInHabitAsync(userId, request.ToDto());
        return CreatedAtAction(
            nameof(GetHabitHistory),
            new { id = request.HabitId },
            ApiResponse<HabitLogDto>.Success(log, "Check-in recorded."));
    }

    [HttpDelete("{id}/checkin")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object>>> UndoCheckIn(int id)
    {
        var userId = await _currentUserService.GetCurrentUserIdAsync();
        var result = await _habitService.UndoCheckInAsync(userId, id);
        if (!result)
            return NotFound(ApiResponse<object>.Fail("No check-in found for today."));
        return Ok(ApiResponse<object>.Success(new { }, "Check-in removed successfully."));
    }

    [HttpGet("{id}/history")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<HabitLogDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<HabitLogDto>>>> GetHabitHistory(
        int id,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        var userId = await _currentUserService.GetCurrentUserIdAsync();
        var logs = await _habitService.GetHabitHistoryAsync(userId, id, startDate, endDate);
        return Ok(ApiResponse<IEnumerable<HabitLogDto>>.Success(logs));
    }

    [HttpGet("{id}/statistics")]
    [ProducesResponseType(typeof(ApiResponse<HabitStatisticsDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<HabitStatisticsDto>>> GetHabitStatistics(int id)
    {
        var userId = await _currentUserService.GetCurrentUserIdAsync();
        var stats = await _habitService.GetHabitStatisticsAsync(userId, id);
        return Ok(ApiResponse<HabitStatisticsDto>.Success(stats));
    }

    [HttpGet("calendar/weekly")]
    [ProducesResponseType(typeof(ApiResponse<WeeklyCalendarDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<WeeklyCalendarDto>>> GetWeeklyCalendar(
        [FromQuery] DateTime? weekStart = null)
    {
        var userId = await _currentUserService.GetCurrentUserIdAsync();
        var start = weekStart ?? GetStartOfWeek(DateTime.UtcNow);
        var calendar = await _habitService.GetWeeklyCalendarAsync(userId, start);
        return Ok(ApiResponse<WeeklyCalendarDto>.Success(calendar));
    }

    private static DateTime GetStartOfWeek(DateTime date)
    {
        int diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
        return date.AddDays(-1 * diff).Date;
    }
}
