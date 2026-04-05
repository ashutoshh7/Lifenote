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
    public bool IsPinned { get; set; }
    public bool IsArchived { get; set; }
}
