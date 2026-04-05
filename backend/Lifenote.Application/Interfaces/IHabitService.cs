using Lifenote.Application.DTOs.Habit;

namespace Lifenote.Application.Interfaces;

/// <summary>
/// Application service contract for Habit use-cases.
/// Moved from Lifenote.Core.Interfaces — canonical home is now Application layer.
/// </summary>
public interface IHabitService
{
    Task<HabitDto> CreateHabitAsync(int userId, CreateHabitDto dto);
    Task<HabitDto> GetHabitByIdAsync(int userId, int habitId);
    Task<IEnumerable<HabitDto>> GetUserHabitsAsync(int userId, bool includeInactive = false);
    Task<HabitDto> UpdateHabitAsync(int userId, int habitId, UpdateHabitDto dto);
    Task<bool> DeleteHabitAsync(int userId, int habitId);
    Task<bool> ToggleHabitStatusAsync(int userId, int habitId);
    Task<HabitLogDto> CheckInHabitAsync(int userId, CheckInDto dto);
    Task<bool> UndoCheckInAsync(int userId, int habitId);
    Task<IEnumerable<HabitLogDto>> GetHabitHistoryAsync(int userId, int habitId, DateTime? startDate = null, DateTime? endDate = null);
    Task<HabitStatisticsDto> GetHabitStatisticsAsync(int userId, int habitId);
    Task<WeeklyCalendarDto> GetWeeklyCalendarAsync(int userId, DateTime weekStart);
}
