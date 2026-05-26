namespace Lifenote.Application.DTOs.Timer;

public class StartTimerDto
{
    public string TimerId { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string SessionType { get; set; } = "pomodoro";
    public int TotalDurationSeconds { get; set; }
}

public class PauseTimerDto
{
    public string TimerId { get; set; } = string.Empty;

    /// <summary>Remaining seconds at the moment of pause, provided by the client.</summary>
    public int RemainingSeconds { get; set; }
}

public class ResetTimerDto
{
    public string TimerId { get; set; } = string.Empty;
}

public class CompleteTimerDto
{
    public string TimerId { get; set; } = string.Empty;

    /// <summary>Actual time spent in seconds (may differ from total if paused).</summary>
    public int ActualDurationSeconds { get; set; }

    public string? Notes { get; set; }
}
