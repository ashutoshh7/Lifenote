namespace Lifenote.Application.Contracts;

/// <summary>
/// Sets the app user id as a Firebase custom claim so future tokens include it (no DB lookup needed).
/// </summary>
public interface IFirebaseClaimService
{
    Task SetAppUserIdClaimAsync(string firebaseUid, Guid appUserId, CancellationToken cancellationToken = default);
}
