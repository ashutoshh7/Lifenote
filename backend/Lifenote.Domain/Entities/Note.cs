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

    /// <summary>
    /// Tags stored as a native PostgreSQL text[] array.
    /// EF Core + Npgsql maps string[] ↔ text[] natively.
    /// </summary>
    public string[] Tags { get; set; } = Array.Empty<string>();

    public bool IsPinned { get; set; }
    public bool IsArchived { get; set; }
}
