namespace Lifenote.Application.Contracts;

/// <summary>
/// Abstracts Firebase custom claim operations.
/// Moved from Lifenote.API/Services — interface now belongs to Application layer.
/// </summary>
public interface IFirebaseClaimService
{
    Task SetAppUserIdClaimAsync(string firebaseUid, int appUserId);
}
