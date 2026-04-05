namespace Lifenote.Domain.Entities;

/// <summary>
/// Core Habit domain entity.
/// No EF Core attributes here — EF config lives in Lifenote.Infrastructure/Persistence/Configurations.
/// </summary>
public class Habit
{
    public int Id { get; set; }
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
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public ICollection<HabitLog> Logs { get; set; } = new List<HabitLog>();
    public HabitStreak? Streak { get; set; }
}
