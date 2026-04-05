using Lifenote.Application.Contracts;
using Lifenote.Application.DTOs.UserInfo;
using Lifenote.Core.Interfaces;
using Lifenote.Core.Models;

namespace Lifenote.Application.Services
{
    public class UserInfoService : IUserInfoService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserInfoService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<UserProfileResponse> CreateUserAsync(string authProviderId, string email, CreateUserDto request)
        {
            if (!await _unitOfWork.Users.IsUsernameAvailableAsync(request.Username))
                throw new InvalidOperationException("Username already taken");
            if (!await _unitOfWork.Users.IsEmailAvailableAsync(email))
                throw new InvalidOperationException("Email already registered");

            var user = new UserInfo
            {
                AuthProviderId = authProviderId,
                Email = email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Username = request.Username,
                DateOfBirth = request.DateOfBirth,
                Theme = "light"
            };

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();
            return MapToResponse(user);
        }

        public async Task<UserProfileResponse> GetUserByAuthIdAsync(string authProviderId)
        {
            var user = await _unitOfWork.Users.GetByAuthProviderIdAsync(authProviderId)
                ?? throw new KeyNotFoundException("User not found");
            return MapToResponse(user);
        }

        public async Task<UserProfileResponse> UpdateProfileAsync(string authProviderId, UpdateProfileDto request)
        {
            var user = await _unitOfWork.Users.GetByAuthProviderIdAsync(authProviderId)
                ?? throw new KeyNotFoundException("User not found");

            if (!string.IsNullOrEmpty(request.FirstName)) user.FirstName = request.FirstName;
            if (!string.IsNullOrEmpty(request.LastName)) user.LastName = request.LastName;
            if (request.DateOfBirth.HasValue) user.DateOfBirth = request.DateOfBirth;
            if (!string.IsNullOrEmpty(request.Bio)) user.Bio = request.Bio;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();
            return MapToResponse(user);
        }

        public async Task UpdateThemeAsync(string authProviderId, string theme)
        {
            var validThemes = new[] { "light", "dark", "auto" };
            if (!validThemes.Contains(theme)) throw new ArgumentException("Invalid theme");

            var user = await _unitOfWork.Users.GetByAuthProviderIdAsync(authProviderId)
                ?? throw new KeyNotFoundException("User not found");
            user.Theme = theme;
            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateProfilePictureAsync(string authProviderId, string profilePictureUrl)
        {
            var user = await _unitOfWork.Users.GetByAuthProviderIdAsync(authProviderId)
                ?? throw new KeyNotFoundException("User not found");
            user.ProfilePicture = profilePictureUrl;
            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateLastLoginAsync(string authProviderId)
        {
            var user = await _unitOfWork.Users.GetByAuthProviderIdAsync(authProviderId)
                ?? throw new KeyNotFoundException("User not found");
            user.LastLoginAt = DateTime.UtcNow;
            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeactivateUserAsync(string authProviderId)
        {
            var user = await _unitOfWork.Users.GetByAuthProviderIdAsync(authProviderId)
                ?? throw new KeyNotFoundException("User not found");
            user.IsActive = false;
            user.DeletedAt = DateTime.UtcNow;
            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();
        }

        public Task<bool> IsUsernameAvailableAsync(string username)
            => _unitOfWork.Users.IsUsernameAvailableAsync(username);

        private static UserProfileResponse MapToResponse(UserInfo user) => new UserProfileResponse
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Username = user.Username,
            DateOfBirth = user.DateOfBirth,
            ProfilePicture = user.ProfilePicture,
            Bio = user.Bio,
            Theme = user.Theme,
            LastLoginAt = user.LastLoginAt
        };
    }
}
