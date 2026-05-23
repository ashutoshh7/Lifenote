namespace Lifenote.Domain.Exceptions;

/// <summary>
/// Thrown when an operation would create a duplicate or violate a uniqueness constraint.
/// ExceptionHandlingMiddleware maps this to HTTP 409 Conflict.
/// </summary>
public class ConflictException : DomainException
{
    public ConflictException(string message) : base(message) { }
}
