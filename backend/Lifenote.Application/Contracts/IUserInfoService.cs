using Lifenote.Application.DTOs.UserInfo;

namespace Lifenote.Application.Contracts;

public interface IUserInfoService
{
    Task<UserProfileResponse> CreateUserAsync(string authProviderId, string email, CreateUserDto request);
    Task<UserProfileResponse> GetUserByAuthIdAsync(string authProviderId);
    Task<UserProfileResponse> UpdateProfileAsync(string authProviderId, UpdateProfileDto request);
    Task UpdateThemeAsync(string authProviderId, string theme);
    Task UpdateProfilePictureAsync(string authProviderId, string profilePictureUrl);
    Task UpdateLastLoginAsync(string authProviderId);
    Task DeactivateUserAsync(string authProviderId);
    Task<bool> IsUsernameAvailableAsync(string username);
}
