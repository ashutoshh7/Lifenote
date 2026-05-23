using Lifenote.Domain.Common;

namespace Lifenote.Domain.Entities;

/// <summary>
/// Note entity. Inherits Id, CreatedAt, UpdatedAt from BaseEntity.
/// </summary>
public class Note : BaseEntity
{
    public int UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Content { get; set; }
    public string? Category { get; set; }

    /// <summary>Comma-separated or JSON tag string — used by NoteService for tag filtering.</summary>
    public string? Tags { get; set; }

    /// <summary>Whether the note is pinned. Plain bool (not nullable) — NoteService treats it as non-nullable.</summary>
    public bool IsPinned { get; set; }

    /// <summary>Whether the note is archived. Plain bool — NoteService treats it as non-nullable.</summary>
    public bool IsArchived { get; set; }
}
