using System.ComponentModel.DataAnnotations;

namespace Lifenote.API.Models.Requests.Note;

/// <summary>
/// HTTP request body for POST /api/notes.
/// This is an API concern — validation attributes live here, not in Application DTOs.
/// The controller maps this to Application.DTOs.Note.CreateNoteDto before calling the service.
/// </summary>
public class CreateNoteRequest
{
    [Required]
    [MaxLength(255)]
    public string Title { get; set; } = string.Empty;

    public string? Content { get; set; }

    [MaxLength(100)]
    public string? Category { get; set; }

    public bool IsPinned { get; set; } = false;
}
