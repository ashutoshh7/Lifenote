using Lifenote.Application.Contracts;
using Lifenote.Domain.Entities;
using Lifenote.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Lifenote.Infrastructure.Repositories;

/// <summary>
/// Moved from Lifenote.Data/Repositories/UserInfoRepository.cs.
/// </summary>
public class UserInfoRepository : IUserInfoRepository
{
    private readonly LifenoteDbContext _db;

    public UserInfoRepository(LifenoteDbContext db) => _db = db;

    public Task<UserInfo?> GetByFirebaseUidAsync(string firebaseUid) =>
        _db.Users.FirstOrDefaultAsync(u => u.FirebaseUid == firebaseUid);

    public Task<UserInfo?> GetByIdAsync(int id) =>
        _db.Users.FirstOrDefaultAsync(u => u.Id == id);

    public Task<bool> IsUsernameAvailableAsync(string username) =>
        _db.Users.AllAsync(u => u.Username != username);

    public async Task<UserInfo> CreateAsync(UserInfo user)
    {
        await _db.Users.AddAsync(user);
        return user;
    }

    public Task<UserInfo> UpdateAsync(UserInfo user)
    {
        _db.Users.Update(user);
        return Task.FromResult(user);
    }
}
