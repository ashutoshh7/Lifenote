namespace Lifenote.Application.DTOs.Habit
{
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
        public List<DailyActivityDto> Last30Days { get; set; } = new();
    }

    public class DailyActivityDto
    {
        public DateTime Date { get; set; }
        public bool Completed { get; set; }
        public string? Notes { get; set; }
    }
}
