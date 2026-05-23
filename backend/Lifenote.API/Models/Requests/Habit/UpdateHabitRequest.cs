using System.ComponentModel.DataAnnotations;

namespace Lifenote.API.Models.Requests.Habit;

/// <summary>
/// HTTP request body for PUT /api/habits/{id}.
/// All validation attributes live here; Application DTOs stay annotation-free.
/// </summary>
public class UpdateHabitRequest
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    [MaxLength(20)]
    public string? Color { get; set; }

    [MaxLength(100)]
    public string? IconName { get; set; }

    [Required]
    [RegularExpression("Daily|Weekly|Custom",
        ErrorMessage = "FrequencyType must be Daily, Weekly, or Custom.")]
    public string FrequencyType { get; set; } = "Daily";

    public string? FrequencyValue { get; set; }

    [Range(1, 100)]
    public int TargetCount { get; set; } = 1;

    public bool IsActive { get; set; } = true;

    public DateTime? EndDate { get; set; }
}
