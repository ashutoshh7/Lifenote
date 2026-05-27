using Lifenote.Domain.Common;

namespace Lifenote.Domain.Entities;

/// <summary>
/// Milestone child entity of the Goal aggregate root.
/// </summary>
public class Milestone : BaseEntity
{
    public Guid GoalId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime? TargetDate { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }

    /// <summary>
    /// Parent Goal navigation property.
    /// </summary>
    public Goal Goal { get; set; } = null!;
}
