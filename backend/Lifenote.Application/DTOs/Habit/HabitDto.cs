namespace Lifenote.Application.DTOs.Habit;

public class HabitDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Color { get; set; }
    public string? IconName { get; set; }
    public string FrequencyType { get; set; } = string.Empty;
    public string? FrequencyValue { get; set; }
    public int TargetCount { get; set; }
    public bool IsActive { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int CurrentStreak { get; set; }
    public int LongestStreak { get; set; }
    public int TotalCompletions { get; set; }
    public bool CompletedToday { get; set; }
    public int CompletedCountToday { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateHabitDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Color { get; set; }
    public string? IconName { get; set; }
    public string FrequencyType { get; set; } = "Daily";
    public string? FrequencyValue { get; set; }
    public int TargetCount { get; set; } = 1;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

public class UpdateHabitDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Color { get; set; }
    public string? IconName { get; set; }
    public string? FrequencyType { get; set; }
    public string? FrequencyValue { get; set; }
    public int? TargetCount { get; set; }
    public bool? IsActive { get; set; }
    public DateTime? EndDate { get; set; }
}

public class CheckInDto
{
    public int HabitId { get; set; }
    public string? Notes { get; set; }
}

public class HabitLogDto
{
    public int Id { get; set; }
    public int HabitId { get; set; }
    public string HabitName { get; set; } = string.Empty;
    public DateTime CompletedAt { get; set; }
    public DateTime CompletedDate { get; set; }
    public string? Notes { get; set; }
    public int CurrentStreak { get; set; }
}

public class HabitStatisticsDto
{
    public int HabitId { get; set; }
    public string HabitName { get; set; } = string.Empty;
    public int CurrentStreak { get; set; }
    public int LongestStreak { get; set; }
    public int TotalCompletions { get; set; }
    public double CompletionRate { get; set; }
    public string? BestDayOfWeek { get; set; }
    public string? WorstDayOfWeek { get; set; }
    public List<DailyActivityDto> Last7Days { get; set; } = new();
}

public class DailyActivityDto
{
    public DateTime Date { get; set; }
    public bool Completed { get; set; }
    public string? Notes { get; set; }
}

public class WeeklyCalendarDto
{
    public DateTime WeekStart { get; set; }
    public DateTime WeekEnd { get; set; }
    public int TotalHabits { get; set; }
    public double OverallCompletionRate { get; set; }
    public List<HabitWeekDto> Habits { get; set; } = new();
}

public class HabitWeekDto
{
    public int HabitId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Color { get; set; }
    public string? IconName { get; set; }
    public string FrequencyType { get; set; } = string.Empty;
    public List<DateTime> CompletedDates { get; set; } = new();
    public int CompletedCount { get; set; }
    public int ExpectedCount { get; set; }
    public double CompletionRate { get; set; }
}
