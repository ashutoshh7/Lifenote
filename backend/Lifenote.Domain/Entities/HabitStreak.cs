namespace Lifenote.Domain.Entities;

public class HabitStreak
{
    public int Id { get; set; }
    public int HabitId { get; set; }
    public int UserId { get; set; }
    public int CurrentStreak { get; set; }
    public int LongestStreak { get; set; }
    public int TotalCompletions { get; set; }
    public DateTime? LastCompletedDate { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Habit? Habit { get; set; }
}
