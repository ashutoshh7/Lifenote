namespace Lifenote.Domain.Entities;

public class HabitLog
{
    public int Id { get; set; }
    public int HabitId { get; set; }
    public int UserId { get; set; }
    public DateTime CompletedAt { get; set; }
    public DateTime CompletedDate { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }

    public Habit? Habit { get; set; }
}
