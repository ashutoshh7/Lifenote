using System.ComponentModel.DataAnnotations;

namespace Lifenote.API.Models.Requests.Habit;

/// <summary>
/// HTTP request body for PUT /api/habits/{id}.
/// All fields are optional (patch semantics).
/// </summary>
public class UpdateHabitRequest
{
    [MaxLength(200)]
    public string? Name { get; set; }

    [MaxLength(1000)]
    public string? Description { get; set; }

    [MaxLength(20)]
    public string? Color { get; set; }

    [MaxLength(100)]
    public string? IconName { get; set; }

    [RegularExpression("Daily|Weekly|Custom")]
    public string? FrequencyType { get; set; }

    public string? FrequencyValue { get; set; }

    [Range(1, 100)]
    public int? TargetCount { get; set; }

    public bool? IsActive { get; set; }
    public DateTime? EndDate { get; set; }
}
