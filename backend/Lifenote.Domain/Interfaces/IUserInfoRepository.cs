using Lifenote.Domain.Entities;

namespace Lifenote.Domain.Interfaces;

/// <summary>
/// Repository interface for UserInfo aggregate root.
/// Defined in Domain — Infrastructure provides the EF Core implementation.
/// </summary>
public interface IUserInfoRepository
{
    Task<UserInfo?> GetByFirebaseUidAsync(string firebaseUid);
    Task<UserInfo?> GetByIdAsync(int id);
    Task<bool> IsUsernameAvailableAsync(string username);
    Task<UserInfo> CreateAsync(UserInfo user);
    Task<UserInfo> UpdateAsync(UserInfo user);
}
