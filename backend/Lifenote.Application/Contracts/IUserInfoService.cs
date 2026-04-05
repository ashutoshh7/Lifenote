using Lifenote.Application.DTOs.UserInfo;

namespace Lifenote.Application.Contracts;

public interface IUserInfoService
{
    Task<UserProfileResponse> CreateUserAsync(string firebaseUid, string email, CreateUserDto dto);
    Task<UserProfileResponse> GetUserByAuthIdAsync(string firebaseUid);
    Task<UserProfileResponse> UpdateProfileAsync(string firebaseUid, UpdateProfileDto dto);
    Task UpdateThemeAsync(string firebaseUid, string theme);
    Task UpdateProfilePictureAsync(string firebaseUid, string profilePictureUrl);
    Task UpdateLastLoginAsync(string firebaseUid);
    Task DeactivateUserAsync(string firebaseUid);
    Task<bool> IsUsernameAvailableAsync(string username);
}
