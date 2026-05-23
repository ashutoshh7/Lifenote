using Lifenote.Domain.Entities;

namespace Lifenote.Domain.Interfaces;

/// <summary>
/// Repository interface for UserInfo aggregate root.
/// Defined in Domain — Infrastructure provides the EF Core implementation.
/// </summary>
public interface IUserInfoRepository
{
    // --- Lookups ---
    Task<UserInfo?> GetByFirebaseUidAsync(string firebaseUid);

    /// <summary>Lookup by the external auth-provider identifier (Firebase UID).
    /// Alias for GetByFirebaseUidAsync — both point to the same field.</summary>
    Task<UserInfo?> GetByAuthProviderIdAsync(string authProviderId);

    Task<UserInfo?> GetByIdAsync(int id);

    // --- Availability checks ---
    Task<bool> IsUsernameAvailableAsync(string username);
    Task<bool> IsEmailAvailableAsync(string email);

    // --- Write operations ---

    /// <summary>Insert a new UserInfo record (returns the tracked entity).</summary>
    Task AddAsync(UserInfo user);

    /// <summary>Canonical async create that returns the persisted entity.</summary>
    Task<UserInfo> CreateAsync(UserInfo user);

    /// <summary>Mark entity as modified in the change tracker (sync, EF Core pattern).</summary>
    void Update(UserInfo user);

    /// <summary>Async update that persists immediately and returns the updated entity.</summary>
    Task<UserInfo> UpdateAsync(UserInfo user);
}
