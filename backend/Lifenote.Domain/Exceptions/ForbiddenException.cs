namespace Lifenote.Domain.Exceptions;

/// <summary>
/// Thrown when the authenticated user does not own the requested resource.
/// ExceptionHandlingMiddleware maps this to HTTP 403 Forbidden.
/// </summary>
public class ForbiddenException : DomainException
{
    public ForbiddenException(string message) : base(message) { }
}
