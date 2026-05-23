namespace Lifenote.Application.DTOs.Habit
{
    public class HabitStreakDto
    {
        public int Id { get; set; }
        public int HabitId { get; set; }
        public string HabitName { get; set; } = null!;
        public int CurrentStreak { get; set; }
        public int LongestStreak { get; set; }
        public int TotalCompletions { get; set; }
        public DateTime? LastCompletedDate { get; set; }
    }
}
