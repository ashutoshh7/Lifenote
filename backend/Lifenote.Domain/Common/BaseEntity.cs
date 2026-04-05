namespace Lifenote.Domain.Common;

/// <summary>
/// Base class for all domain entities.
/// Provides a strongly-typed Id and common audit timestamps.
/// </summary>
public abstract class BaseEntity<TId>
{
    public TId Id { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Convenience alias for the most common int-keyed entity.
/// </summary>
public abstract class BaseEntity : BaseEntity<int> { }
