using System.ComponentModel.DataAnnotations;

namespace Lifenote.API.Models.Requests.Note;

/// <summary>
/// HTTP request body for PUT /api/note/{id}.
/// Uses nullable booleans for optional patch semantics (GetValueOrDefault used in mapping).
/// </summary>
public class UpdateNoteRequest
{
    [Required]
    [MaxLength(255)]
    public string? Title { get; set; }

    public string? Content { get; set; }

    [MaxLength(100)]
    public string? Category { get; set; }

    public bool? IsPinned { get; set; }

    public bool? IsArchived { get; set; }
}
