using System.ComponentModel.DataAnnotations;

namespace Lifenote.API.Models.Requests.FocusSession;

/// <summary>
/// HTTP request body for POST /api/focus-sessions.
/// </summary>
public class CreateFocusSessionRequest
{
    [Required]
    public DateTime StartedAt { get; set; }

    public DateTime? EndedAt { get; set; }

    [Range(1, 1440, ErrorMessage = "Duration must be between 1 and 1440 minutes (24 hours).")]
    public int DurationMinutes { get; set; }

    [MaxLength(1000)]
    public string? Notes { get; set; }
}
