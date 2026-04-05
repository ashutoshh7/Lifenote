using FirebaseAdmin.Auth;
using Lifenote.Application.Contracts;

namespace Lifenote.Infrastructure.Services;

/// <summary>
/// Moved from Lifenote.API/Services/FirebaseClaimService.cs.
/// External service calls (Firebase Admin SDK) belong in Infrastructure.
/// Interface IFirebaseClaimService moved to Application.Contracts.
/// </summary>
public class FirebaseClaimService : IFirebaseClaimService
{
    public async Task SetAppUserIdClaimAsync(string firebaseUid, int appUserId)
    {
        try
        {
            var claims = new Dictionary<string, object> { { "app_user_id", appUserId } };
            await FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(firebaseUid, claims);
        }
        catch
        {
            // Firebase Admin not configured — graceful degradation
        }
    }
}
