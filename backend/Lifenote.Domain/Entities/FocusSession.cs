using Lifenote.Domain.Common;

namespace Lifenote.Domain.Entities;

/// <summary>
/// FocusSession entity (Pomodoro sessions).
/// Inherits Id, CreatedAt, UpdatedAt from BaseEntity.
/// </summary>
public class FocusSession : BaseEntity
{
    public int UserId { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }
    public int DurationMinutes { get; set; }
    public string? Notes { get; set; }
}
