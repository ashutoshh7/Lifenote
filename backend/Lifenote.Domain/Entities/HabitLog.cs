using Lifenote.Domain.Common;

namespace Lifenote.Domain.Entities;

/// <summary>
/// HabitLog is a child entity of the Habit aggregate.
/// It should only be created/deleted through the Habit aggregate root.
/// Inherits Id, CreatedAt, UpdatedAt from BaseEntity.
/// </summary>
public class HabitLog : BaseEntity
{
    public int HabitId { get; set; }
    public int UserId { get; set; }
    public DateTime CompletedAt { get; set; }
    public DateTime CompletedDate { get; set; }
    public string? Notes { get; set; }

    // Navigation back to aggregate root
    public Habit? Habit { get; set; }
}
