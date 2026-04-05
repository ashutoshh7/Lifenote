namespace Lifenote.API.Models.Requests.Habit;

/// <summary>
/// HTTP request body for POST /api/habits/{id}/checkin.
/// </summary>
public class CheckInRequest
{
    public int HabitId { get; set; }
    public string? Notes { get; set; }
    public DateTime? CompletedAt { get; set; } // defaults to UtcNow if null
}
