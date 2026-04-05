using Lifenote.Application.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;

namespace Lifenote.Infrastructure.Services;

/// <summary>
/// Moved from Lifenote.API/Services/CurrentUserService.cs.
/// Infrastructure is the correct home for services that depend on
/// ASP.NET Core HttpContext and external concerns (cache, DB lookup).
/// The interface ICurrentUserService lives in Application.
/// </summary>
public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMemoryCache _cache;
    private readonly IUserInfoRepository _userInfoRepository;

    public CurrentUserService(
        IHttpContextAccessor httpContextAccessor,
        IMemoryCache cache,
        IUserInfoRepository userInfoRepository)
    {
        _httpContextAccessor = httpContextAccessor;
        _cache = cache;
        _userInfoRepository = userInfoRepository;
    }

    public string? GetFirebaseUid() =>
        _httpContextAccessor.HttpContext?.User.FindFirst("user_id")?.Value;

    public async Task<int> GetCurrentUserIdAsync()
    {
        var firebaseUid = GetFirebaseUid()
            ?? throw new UnauthorizedAccessException("Firebase UID missing from token");

        var cacheKey = $"uid_to_appid_{firebaseUid}";
        if (_cache.TryGetValue(cacheKey, out int cachedId))
            return cachedId;

        var user = await _userInfoRepository.GetByFirebaseUidAsync(firebaseUid)
            ?? throw new UnauthorizedAccessException("User not found in database");

        _cache.Set(cacheKey, user.Id, TimeSpan.FromMinutes(30));
        return user.Id;
    }
}
