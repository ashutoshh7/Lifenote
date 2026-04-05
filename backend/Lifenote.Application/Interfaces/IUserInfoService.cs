using Lifenote.Application.DTOs.UserInfo;

namespace Lifenote.Application.Interfaces;

/// <summary>
/// Application service contract for UserInfo use-cases.
/// Moved from Lifenote.Core.Interfaces — canonical home is now Application layer.
/// Note: method signatures use Application DTOs only, not Core DTOs.
/// </summary>
public interface IUserInfoService
{
    Task<UserProfileDto> CreateUserAsync(string authProviderId, string email, CreateUserDto request);
    Task<UserProfileDto> GetUserByAuthIdAsync(string authProviderId);
    Task<UserProfileDto> UpdateProfileAsync(string authProviderId, UpdateProfileDto request);
    Task UpdateThemeAsync(string authProviderId, string theme);
    Task UpdateProfilePictureAsync(string authProviderId, string profilePictureUrl);
    Task UpdateLastLoginAsync(string authProviderId);
    Task DeactivateUserAsync(string authProviderId);
    Task<bool> IsUsernameAvailableAsync(string username);
}
