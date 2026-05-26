namespace Lifenote.Application.DTOs.Timer;

public class ActiveTimerDto
{
    public string TimerId { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string SessionType { get; set; } = string.Empty;

    /// <summary>"idle" | "running" | "paused"</summary>
    public string Status { get; set; } = "idle";

    public int TotalDurationSeconds { get; set; }

    /// <summary>
    /// UTC time when timer was last started/resumed.
    /// Present only when Status = "running".
    /// Frontend calculates: remaining = (StartedAt + TotalDurationSeconds) - UtcNow
    /// </summary>
    public DateTimeOffset? StartedAt { get; set; }

    /// <summary>
    /// Frozen remaining seconds. Present only when Status = "paused".
    /// </summary>
    public int? RemainingSeconds { get; set; }
}
