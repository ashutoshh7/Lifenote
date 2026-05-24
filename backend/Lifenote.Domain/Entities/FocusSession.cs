using Lifenote.Domain.Common;

namespace Lifenote.Domain.Entities;

/// <summary>
/// FocusSession entity (Pomodoro/focus sessions).
/// Inherits Id, CreatedAt from BaseEntity.
/// Note: DB table (FocusSessions) has no UpdatedAt column — ignored in configuration.
/// </summary>
public class FocusSession : BaseEntity
{
    public int UserId { get; set; }

    /// <summary>e.g. "Pomodoro", "ShortBreak", "LongBreak"</summary>
    public string SessionType { get; set; } = string.Empty;

    /// <summary>Planned duration in seconds.</summary>
    public int Duration { get; set; }

    /// <summary>Actual time spent in seconds (null if session was never started).</summary>
    public int? ActualDuration { get; set; }

    public DateTimeOffset? StartTime { get; set; }
    public DateTimeOffset? EndTime { get; set; }
    public bool IsCompleted { get; set; }
    public string? Notes { get; set; }
}
