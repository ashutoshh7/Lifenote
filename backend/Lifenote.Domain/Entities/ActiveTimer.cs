using Lifenote.Domain.Common;

namespace Lifenote.Domain.Entities;

/// <summary>
/// Represents the live state of a running/paused Pomodoro timer.
/// One row per (UserId, TimerId) pair — this is a scratchpad, not history.
/// On session complete, this row is deleted and a FocusSession record is written.
/// Status: "idle" | "running" | "paused"
/// </summary>
public class ActiveTimer : BaseEntity
{
    public Guid UserId { get; set; }

    /// <summary>Frontend-generated timer id (e.g. "pomo-abc123").</summary>
    public string TimerId { get; set; } = string.Empty;

    public string Label { get; set; } = string.Empty;

    /// <summary>e.g. "pomodoro", "short-break", "long-break"</summary>
    public string SessionType { get; set; } = string.Empty;

    /// <summary>"idle" | "running" | "paused"</summary>
    public string Status { get; set; } = "idle";

    /// <summary>Total planned duration in seconds.</summary>
    public int TotalDurationSeconds { get; set; }

    /// <summary>
    /// UTC timestamp when the timer was last started/resumed.
    /// Null when paused or idle. Frontend uses this to calculate remaining time:
    /// remaining = (StartedAt + TotalDurationSeconds) - UtcNow
    /// </summary>
    public DateTimeOffset? StartedAt { get; set; }

    /// <summary>
    /// Seconds remaining at the moment of pause.
    /// Null when running. Set by the client when calling PauseTimer.
    /// </summary>
    public int? RemainingSeconds { get; set; }
}
