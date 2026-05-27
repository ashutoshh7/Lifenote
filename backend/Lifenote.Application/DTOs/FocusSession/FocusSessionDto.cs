namespace Lifenote.Application.DTOs.FocusSession;

public class FocusSessionDto
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int DurationMinutes { get; set; }
    public string SessionType { get; set; } = string.Empty; // "pomodoro", "short-break", "long-break"
    public string? Label { get; set; }
    public string? Notes { get; set; }
}
