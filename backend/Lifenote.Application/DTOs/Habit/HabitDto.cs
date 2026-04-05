namespace Lifenote.Application.DTOs.Habit
{
    public class HabitDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Color { get; set; } = string.Empty;
        public string IconName { get; set; } = string.Empty;
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
}
