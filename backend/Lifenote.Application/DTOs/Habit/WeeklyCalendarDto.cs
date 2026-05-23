namespace Lifenote.Application.DTOs.Habit
{
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
        public string Color { get; set; } = string.Empty;
        public string IconName { get; set; } = string.Empty;
        public string FrequencyType { get; set; } = string.Empty;
        public List<DateTime> CompletedDates { get; set; } = new();
        public int CompletedCount { get; set; }
        public int ExpectedCount { get; set; }
        public double CompletionRate { get; set; }
    }
}
