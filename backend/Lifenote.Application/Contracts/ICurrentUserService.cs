namespace Lifenote.Application.Contracts;

/// <summary>
/// Abstracts retrieval of the current authenticated user's internal DB id.
/// Implementation lives in Lifenote.Infrastructure (CurrentUserService).
/// </summary>
public interface ICurrentUserService
{
    Task<int> GetCurrentUserIdAsync();
    string? GetFirebaseUid();
}
