using Lifenote.Domain.Common;

namespace Lifenote.Domain.Entities;

/// <summary>
/// HabitStreak is a child entity of the Habit aggregate.
/// Inherits Id, CreatedAt, UpdatedAt from BaseEntity.
/// </summary>
public class HabitStreak : BaseEntity
{
    public int HabitId { get; set; }
    public int UserId { get; set; }
    public int CurrentStreak { get; set; }
    public int LongestStreak { get; set; }
    public int TotalCompletions { get; set; }
    public DateTime? LastCompletedDate { get; set; }

    /// <summary>
    /// Timestamp of the last time this streak was recalculated.
    /// Distinct from UpdatedAt (BaseEntity) which tracks EF row changes.
    /// </summary>
    public DateTime? CalculatedAt { get; set; }

    // Navigation back to aggregate root
    public Habit? Habit { get; set; }
}
