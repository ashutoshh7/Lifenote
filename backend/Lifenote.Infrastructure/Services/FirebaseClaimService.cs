using FirebaseAdmin.Auth;
using Lifenote.Application.Contracts;

namespace Lifenote.Infrastructure.Services;

public class FirebaseClaimService : IFirebaseClaimService
{
    public async Task SetAppUserIdClaimAsync(string firebaseUid, int appUserId, CancellationToken cancellationToken = default)
    {
        try
        {
            var auth = FirebaseAuth.DefaultInstance;
            if (auth == null) return;

            var claims = new Dictionary<string, object> { { "app_user_id", appUserId } };
            await auth.SetCustomUserClaimsAsync(firebaseUid, claims, cancellationToken);
        }
        catch (Exception)
        {
            // Firebase not configured or claim failed — cache/DB fallback will handle it
        }
    }
}
