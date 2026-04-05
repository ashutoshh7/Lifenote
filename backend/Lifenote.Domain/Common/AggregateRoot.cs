namespace Lifenote.Domain.Common;

/// <summary>
/// Marker base class for Aggregate Roots.
/// An Aggregate Root is the only entry point for modifying
/// the aggregate's child entities (e.g. Habit controls HabitLog and HabitStreak).
/// Domain events can be collected here in a future iteration.
/// </summary>
public abstract class AggregateRoot<TId> : BaseEntity<TId> { }

/// <summary>
/// Convenience alias for int-keyed aggregate roots.
/// </summary>
public abstract class AggregateRoot : AggregateRoot<int> { }
