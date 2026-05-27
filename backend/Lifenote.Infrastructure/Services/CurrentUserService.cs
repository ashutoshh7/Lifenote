using System.Security.Claims;
using Lifenote.Application.Contracts;
using Lifenote.Application.DTOs.UserInfo;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;

namespace Lifenote.Infrastructure.Services;

/// <summary>
/// Resolves current app user id from JWT: "app_user_id" custom claim first,
/// then in-memory cache (Firebase UID -> app user id), then single DB lookup.
/// </summary>
public class CurrentUserService : ICurrentUserService
{
    public const string AppUserIdClaim = "app_user_id";
    public const string CacheKeyPrefix = "uid:";
    public static readonly TimeSpan CacheExpiration = TimeSpan.FromMinutes(15);

    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserInfoService _userInfoService;
    private readonly IMemoryCache _cache;
    private readonly IFirebaseClaimService _firebaseClaimService;

    public CurrentUserService(
        IHttpContextAccessor httpContextAccessor,
        IUserInfoService userInfoService,
        IMemoryCache cache,
        IFirebaseClaimService firebaseClaimService)
    {
        _httpContextAccessor = httpContextAccessor;
        _userInfoService = userInfoService;
        _cache = cache;
        _firebaseClaimService = firebaseClaimService;
    }

    public async Task<Guid> GetCurrentUserIdAsync(CancellationToken cancellationToken = default)
    {
        var user = _httpContextAccessor.HttpContext?.User
            ?? throw new UnauthorizedAccessException("Not authenticated");

        // 1. From custom claim (no DB, no cache)
        var appUserIdClaim = user.FindFirst(AppUserIdClaim)?.Value;
        if (!string.IsNullOrEmpty(appUserIdClaim) && Guid.TryParse(appUserIdClaim, out Guid fromClaim))
            return fromClaim;

        // 2. From cache
        var firebaseUid = user.FindFirst("user_id")?.Value ?? user.FindFirst("sub")?.Value;
        if (string.IsNullOrEmpty(firebaseUid))
            throw new UnauthorizedAccessException("Invalid token: missing user identifier");

        var cacheKey = CacheKeyPrefix + firebaseUid;
        if (_cache.TryGetValue(cacheKey, out Guid cachedId))
            return cachedId;

        // 3. From DB (once per cache TTL)
        UserProfileResponse profile;
        try
        {
            profile = await _userInfoService.GetUserByAuthIdAsync(firebaseUid);
        }
        catch (KeyNotFoundException)
        {
            throw new UnauthorizedAccessException("User not found");
        }

        var appUserId = profile.Id;
        _cache.Set(cacheKey, appUserId, CacheExpiration);
        await _firebaseClaimService.SetAppUserIdClaimAsync(firebaseUid, appUserId, cancellationToken);

        return appUserId;
    }
}
