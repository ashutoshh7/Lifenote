using Lifenote.Application.Contracts;
using Lifenote.Application.DTOs.UserInfo;
using Lifenote.Domain.Entities;

namespace Lifenote.Application.Services;

/// <summary>
/// Moved from Lifenote.Data/Services/UserInfoService.cs.
/// </summary>
public class UserInfoService : IUserInfoService
{
    private readonly IUnitOfWork _unitOfWork;

    public UserInfoService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<UserProfileResponse> CreateUserAsync(string firebaseUid, string email, CreateUserDto dto)
    {
        var user = new UserInfo
        {
            FirebaseUid = firebaseUid,
            Email = email,
            Username = dto.Username,
            DisplayName = dto.DisplayName,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await _unitOfWork.Users.CreateAsync(user);
        await _unitOfWork.SaveChangesAsync();
        return MapToResponse(user);
    }

    public async Task<UserProfileResponse> GetUserByAuthIdAsync(string firebaseUid)
    {
        var user = await _unitOfWork.Users.GetByFirebaseUidAsync(firebaseUid)
            ?? throw new KeyNotFoundException("User not found");
        return MapToResponse(user);
    }

    public async Task<UserProfileResponse> UpdateProfileAsync(string firebaseUid, UpdateProfileDto dto)
    {
        var user = await _unitOfWork.Users.GetByFirebaseUidAsync(firebaseUid)
            ?? throw new KeyNotFoundException("User not found");
        if (dto.Username != null) user.Username = dto.Username;
        if (dto.DisplayName != null) user.DisplayName = dto.DisplayName;
        user.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.Users.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();
        return MapToResponse(user);
    }

    public async Task UpdateThemeAsync(string firebaseUid, string theme)
    {
        var user = await _unitOfWork.Users.GetByFirebaseUidAsync(firebaseUid)
            ?? throw new KeyNotFoundException("User not found");
        user.Theme = theme;
        user.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.Users.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateProfilePictureAsync(string firebaseUid, string profilePictureUrl)
    {
        var user = await _unitOfWork.Users.GetByFirebaseUidAsync(firebaseUid)
            ?? throw new KeyNotFoundException("User not found");
        user.ProfilePictureUrl = profilePictureUrl;
        user.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.Users.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateLastLoginAsync(string firebaseUid)
    {
        var user = await _unitOfWork.Users.GetByFirebaseUidAsync(firebaseUid)
            ?? throw new KeyNotFoundException("User not found");
        user.LastLoginAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.Users.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeactivateUserAsync(string firebaseUid)
    {
        var user = await _unitOfWork.Users.GetByFirebaseUidAsync(firebaseUid)
            ?? throw new KeyNotFoundException("User not found");
        user.IsActive = false;
        user.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.Users.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();
    }

    public Task<bool> IsUsernameAvailableAsync(string username) =>
        _unitOfWork.Users.IsUsernameAvailableAsync(username);

    private static UserProfileResponse MapToResponse(UserInfo user) => new()
    {
        Id = user.Id,
        FirebaseUid = user.FirebaseUid,
        Email = user.Email,
        Username = user.Username,
        DisplayName = user.DisplayName,
        ProfilePictureUrl = user.ProfilePictureUrl,
        Theme = user.Theme,
        IsActive = user.IsActive,
        CreatedAt = user.CreatedAt,
        LastLoginAt = user.LastLoginAt
    };
}
