using Lifenote.Domain.Common;

namespace Lifenote.Domain.Entities;

/// <summary>
/// UserInfo aggregate root — represents the application user profile.
/// Inherits Id, CreatedAt, UpdatedAt from AggregateRoot (via BaseEntity).
/// </summary>
public class UserInfo : AggregateRoot
{
    /// <summary>Firebase UID — the external auth provider identifier.</summary>
    public string FirebaseUid { get; set; } = string.Empty;

    /// <summary>Alias used by services that received the token claim as 'authProviderId'.</summary>
    public string AuthProviderId
    {
        get => FirebaseUid;
        set => FirebaseUid = value;
    }

    public string Email { get; set; } = string.Empty;
    public string? Username { get; set; }

    // --- Profile fields expected by UserInfoService ---
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Bio { get; set; }

    /// <summary>Profile picture URL (canonical name used by service).</summary>
    public string? ProfilePicture { get; set; }

    /// <summary>Convenience alias that maps to ProfilePicture for callers using the old name.</summary>
    public string? ProfilePictureUrl
    {
        get => ProfilePicture;
        set => ProfilePicture = value;
    }

    public string? DisplayName { get; set; }
    public string? Theme { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? LastLoginAt { get; set; }

    /// <summary>Soft-delete timestamp set by DeactivateUserAsync.</summary>
    public DateTime? DeletedAt { get; set; }
}
