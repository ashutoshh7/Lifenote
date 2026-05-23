namespace Lifenote.Domain.Exceptions;

/// <summary>
/// Thrown when a requested resource does not exist or is not visible to the caller.
/// ExceptionHandlingMiddleware maps this to HTTP 404 Not Found.
/// </summary>
public class NotFoundException : DomainException
{
    public NotFoundException(string message) : base(message) { }

    /// <summary>Convenience factory: "Habit 42 not found."</summary>
    public static NotFoundException For(string resource, object id)
        => new($"{resource} {id} not found.");
}
