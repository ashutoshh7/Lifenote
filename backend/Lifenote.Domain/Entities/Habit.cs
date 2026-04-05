using Lifenote.Domain.Common;

namespace Lifenote.Domain.Entities;

/// <summary>
/// Habit aggregate root. Controls access to HabitLog and HabitStreak.
/// Inherits Id, CreatedAt, UpdatedAt from AggregateRoot (via BaseEntity).
/// No EF Core attributes here — EF config lives in Lifenote.Infrastructure/Persistence/Configurations.
/// </summary>
public class Habit : AggregateRoot
{
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Color { get; set; }
    public string? IconName { get; set; }
    public string FrequencyType { get; set; } = "Daily";
    public string? FrequencyValue { get; set; }
    public int TargetCount { get; set; } = 1;
    public bool IsActive { get; set; } = true;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    // Navigation properties — child entities of this aggregate
    public ICollection<HabitLog> Logs { get; set; } = new List<HabitLog>();
    public HabitStreak? Streak { get; set; }
}
