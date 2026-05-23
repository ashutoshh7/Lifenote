using Lifenote.Domain.Entities;
using Lifenote.Domain.Interfaces;
using Lifenote.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Lifenote.Infrastructure.Repositories;

public class UserInfoRepository : IUserInfoRepository
{
    private readonly LifenoteDbContext _db;

    public UserInfoRepository(LifenoteDbContext db) => _db = db;

    // AuthProviderId is an ignored alias on the entity — always query via FirebaseUid
    public Task<UserInfo?> GetByFirebaseUidAsync(string firebaseUid) =>
        _db.UserInfos.FirstOrDefaultAsync(u => u.FirebaseUid == firebaseUid);

    public Task<UserInfo?> GetByAuthProviderIdAsync(string authProviderId) =>
        _db.UserInfos.FirstOrDefaultAsync(u => u.FirebaseUid == authProviderId);

    public Task<UserInfo?> GetByIdAsync(int id) =>
        _db.UserInfos.FirstOrDefaultAsync(u => u.Id == id);

    public Task<bool> IsUsernameAvailableAsync(string username) =>
        _db.UserInfos.AllAsync(u => u.Username != username);

    public Task<bool> IsEmailAvailableAsync(string email) =>
        _db.UserInfos.AllAsync(u => u.Email != email);

    public async Task AddAsync(UserInfo user) =>
        await _db.UserInfos.AddAsync(user);

    public async Task<UserInfo> CreateAsync(UserInfo user)
    {
        await _db.UserInfos.AddAsync(user);
        return user;
    }

    public void Update(UserInfo user) =>
        _db.UserInfos.Update(user);

    public Task<UserInfo> UpdateAsync(UserInfo user)
    {
        _db.UserInfos.Update(user);
        return Task.FromResult(user);
    }
}
