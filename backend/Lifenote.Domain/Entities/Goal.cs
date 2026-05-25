using Lifenote.Domain.Common;

namespace Lifenote.Domain.Entities;

/// <summary>
/// Goal aggregate root. Represents a high-level user goal.
/// </summary>
public class Goal : AggregateRoot
{
    public int UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime? TargetDate { get; set; }
    
    /// <summary>
    /// e.g. "Pending", "InProgress", "Completed", "Abandoned"
    /// </summary>
    public string Status { get; set; } = "Pending";
    public string? Category { get; set; }

    /// <summary>
    /// Milestones belonging to this goal.
    /// </summary>
    public ICollection<Milestone> Milestones { get; set; } = new List<Milestone>();
}
