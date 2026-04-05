using Lifenote.Domain.Entities;

namespace Lifenote.Application.Contracts;

public interface IUserInfoRepository
{
    Task<UserInfo?> GetByFirebaseUidAsync(string firebaseUid);
    Task<UserInfo?> GetByIdAsync(int id);
    Task<bool> IsUsernameAvailableAsync(string username);
    Task<UserInfo> CreateAsync(UserInfo user);
    Task<UserInfo> UpdateAsync(UserInfo user);
}
