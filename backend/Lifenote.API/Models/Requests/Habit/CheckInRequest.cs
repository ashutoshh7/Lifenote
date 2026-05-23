using System.ComponentModel.DataAnnotations;

namespace Lifenote.API.Models.Requests.Habit;

/// <summary>
/// HTTP request body for POST /api/habits/checkin.
/// </summary>
public class CheckInRequest
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "HabitId must be a positive integer.")]
    public int HabitId { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }
}
