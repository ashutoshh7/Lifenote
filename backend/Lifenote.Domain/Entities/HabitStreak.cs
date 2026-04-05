using Lifenote.Domain.Common;

namespace Lifenote.Domain.Entities;

/// <summary>
/// HabitStreak is a child entity of the Habit aggregate.
/// Inherits Id from BaseEntity. Note: UpdatedAt is retained from BaseEntity.
/// CreatedAt is not applicable here — streak is recalculated, not created once.
/// </summary>
public class HabitStreak : BaseEntity
{
    public int HabitId { get; set; }
    public int UserId { get; set; }
    public int CurrentStreak { get; set; }
    public int LongestStreak { get; set; }
    public int TotalCompletions { get; set; }
    public DateTime? LastCompletedDate { get; set; }

    // Navigation back to aggregate root
    public Habit? Habit { get; set; }
}
