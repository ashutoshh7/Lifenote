namespace Lifenote.Application.Interfaces;

/// <summary>
/// Resolves the current authenticated user's integer DB id from the Firebase JWT.
/// Moved from Lifenote.Core.Interfaces — canonical home is now Application layer.
/// Implementation lives in Lifenote.API/Services/CurrentUserService.cs (infrastructure concern).
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// Returns the current user's integer DB id.
    /// Throws UnauthorizedAccessException if not authenticated.
    /// Throws KeyNotFoundException if user is authenticated but not found in DB.
    /// </summary>
    Task<int> GetCurrentUserIdAsync(CancellationToken cancellationToken = default);
}
