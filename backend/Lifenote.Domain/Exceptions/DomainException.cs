namespace Lifenote.Domain.Exceptions;

/// <summary>
/// Base class for all domain-level exceptions.
/// ExceptionHandlingMiddleware maps these to HTTP 400 Bad Request by default.
/// Use a derived type for a specific HTTP status code.
/// </summary>
public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
    public DomainException(string message, Exception inner) : base(message, inner) { }
}
