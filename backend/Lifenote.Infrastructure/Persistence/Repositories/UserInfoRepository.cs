using Lifenote.Core.Interfaces;
using Lifenote.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Lifenote.Infrastructure.Persistence.Repositories
{
    public class UserInfoRepository : IUserInfoRepository
    {
        private readonly LifenoteDbContext _context;

        public UserInfoRepository(LifenoteDbContext context)
        {
            _context = context;
        }

        public async Task<UserInfo> GetByIdAsync(int id)
            => await _context.UserInfos
                .FirstOrDefaultAsync(u => u.Id == id && u.IsActive == true && u.DeletedAt == null);

        public async Task<UserInfo> GetByAuthProviderIdAsync(string authProviderId)
            => await _context.UserInfos
                .FirstOrDefaultAsync(u => u.AuthProviderId == authProviderId && u.IsActive == true && u.DeletedAt == null);

        public async Task<UserInfo> GetByUsernameAsync(string username)
        {
            var normalized = username.ToLower();
            return await _context.UserInfos
                .FirstOrDefaultAsync(u => u.IsActive == true && u.DeletedAt == null &&
                                          u.Username.ToLower() == normalized);
        }

        public async Task<UserInfo> GetByEmailAsync(string email)
        {
            var normalized = email.ToLower();
            return await _context.UserInfos
                .FirstOrDefaultAsync(u => u.IsActive == true && u.DeletedAt == null &&
                                          u.Email.ToLower() == normalized);
        }

        public async Task<bool> IsUsernameAvailableAsync(string username)
        {
            var normalized = username.ToLower();
            return !await _context.UserInfos
                .AnyAsync(u => u.Username.ToLower() == normalized && u.DeletedAt == null);
        }

        public async Task<bool> IsEmailAvailableAsync(string email)
        {
            var normalized = email.ToLower();
            return !await _context.UserInfos
                .AnyAsync(u => u.Email.ToLower() == normalized && u.DeletedAt == null);
        }

        public async Task AddAsync(UserInfo user)
        {
            user.CreatedAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;
            user.IsActive = true;
            await _context.UserInfos.AddAsync(user);
        }

        public void Update(UserInfo user)
        {
            user.UpdatedAt = DateTime.UtcNow;
            _context.UserInfos.Update(user);
        }
    }
}
