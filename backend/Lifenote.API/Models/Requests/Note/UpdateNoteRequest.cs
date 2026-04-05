using System.ComponentModel.DataAnnotations;

namespace Lifenote.API.Models.Requests.Note;

/// <summary>
/// HTTP request body for PUT /api/notes/{id}.
/// Nullable fields = optional update (patch semantics).
/// </summary>
public class UpdateNoteRequest
{
    [MaxLength(255)]
    public string? Title { get; set; }

    public string? Content { get; set; }

    [MaxLength(100)]
    public string? Category { get; set; }

    public bool? IsPinned { get; set; }
    public bool? IsArchived { get; set; }
}
