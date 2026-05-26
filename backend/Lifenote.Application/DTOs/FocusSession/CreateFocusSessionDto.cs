namespace Lifenote.Application.DTOs.FocusSession;

public class CreateFocusSessionDto
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int DurationMinutes { get; set; }
    public string SessionType { get; set; } = string.Empty;
    public string? Label { get; set; }
    public string? Notes { get; set; }
}
