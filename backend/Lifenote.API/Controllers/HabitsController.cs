using Lifenote.Application.Contracts;
using Lifenote.Application.DTOs.Habit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lifenote.API.Controllers;

/// <summary>
/// Refactored: controllers are now thin.
/// - No try/catch blocks — ExceptionHandlingMiddleware handles all exceptions globally.
/// - Namespace updated from Lifenote.Core.DTOs to Lifenote.Application.DTOs.
/// - ICurrentUserService interface is now from Application.Contracts.
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
    [ProducesResponseType(typeof(IEnumerable<HabitDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<HabitDto>>> GetHabits([FromQuery] bool includeInactive = false)
    {
        var userId = await _currentUserService.GetCurrentUserIdAsync();
        var habits = await _habitService.GetUserHabitsAsync(userId, includeInactive);
        return Ok(habits);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(HabitDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<HabitDto>> GetHabit(int id)
    {
        var userId = await _currentUserService.GetCurrentUserIdAsync();
        var habit = await _habitService.GetHabitByIdAsync(userId, id);
        return Ok(habit);
    }

    [HttpPost]
    [ProducesResponseType(typeof(HabitDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<HabitDto>> CreateHabit([FromBody] CreateHabitDto dto)
    {
        var userId = await _currentUserService.GetCurrentUserIdAsync();
        var habit = await _habitService.CreateHabitAsync(userId, dto);
        return Ok(habit);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(HabitDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<HabitDto>> UpdateHabit(int id, [FromBody] UpdateHabitDto dto)
    {
        var userId = await _currentUserService.GetCurrentUserIdAsync();
        var habit = await _habitService.UpdateHabitAsync(userId, id, dto);
        return Ok(habit);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteHabit(int id)
    {
        var userId = await _currentUserService.GetCurrentUserIdAsync();
        var result = await _habitService.DeleteHabitAsync(userId, id);
        if (!result) return NotFound(new { message = "Habit not found" });
        return NoContent();
    }

    [HttpPatch("{id}/toggle")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ToggleHabitStatus(int id)
    {
        var userId = await _currentUserService.GetCurrentUserIdAsync();
        var result = await _habitService.ToggleHabitStatusAsync(userId, id);
        if (!result) return NotFound(new { message = "Habit not found" });
        return Ok(new { message = "Habit status toggled successfully" });
    }

    [HttpPost("checkin")]
    [ProducesResponseType(typeof(HabitLogDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<HabitLogDto>> CheckIn([FromBody] CheckInDto dto)
    {
        var userId = await _currentUserService.GetCurrentUserIdAsync();
        var log = await _habitService.CheckInHabitAsync(userId, dto);
        return CreatedAtAction(nameof(GetHabitHistory), new { id = dto.HabitId }, log);
    }

    [HttpDelete("{id}/checkin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UndoCheckIn(int id)
    {
        var userId = await _currentUserService.GetCurrentUserIdAsync();
        var result = await _habitService.UndoCheckInAsync(userId, id);
        if (!result) return NotFound(new { message = "No check-in found for today" });
        return Ok(new { message = "Check-in removed successfully" });
    }

    [HttpGet("{id}/history")]
    [ProducesResponseType(typeof(IEnumerable<HabitLogDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<HabitLogDto>>> GetHabitHistory(
        int id,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        var userId = await _currentUserService.GetCurrentUserIdAsync();
        var logs = await _habitService.GetHabitHistoryAsync(userId, id, startDate, endDate);
        return Ok(logs);
    }

    [HttpGet("{id}/statistics")]
    [ProducesResponseType(typeof(HabitStatisticsDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<HabitStatisticsDto>> GetHabitStatistics(int id)
    {
        var userId = await _currentUserService.GetCurrentUserIdAsync();
        var stats = await _habitService.GetHabitStatisticsAsync(userId, id);
        return Ok(stats);
    }

    [HttpGet("calendar/weekly")]
    [ProducesResponseType(typeof(WeeklyCalendarDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<WeeklyCalendarDto>> GetWeeklyCalendar([FromQuery] DateTime? weekStart = null)
    {
        var userId = await _currentUserService.GetCurrentUserIdAsync();
        var start = weekStart ?? GetStartOfWeek(DateTime.UtcNow);
        var calendar = await _habitService.GetWeeklyCalendarAsync(userId, start);
        return Ok(calendar);
    }

    private static DateTime GetStartOfWeek(DateTime date)
    {
        int diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
        return date.AddDays(-1 * diff).Date;
    }
}
