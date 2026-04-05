namespace Lifenote.Domain.Entities;

public class FocusSession
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }
    public int DurationMinutes { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}
