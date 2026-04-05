using Lifenote.Domain.Common;

namespace Lifenote.Domain.Entities;

/// <summary>
/// UserInfo aggregate root — represents the application user profile.
/// Inherits Id, CreatedAt, UpdatedAt from AggregateRoot (via BaseEntity).
/// </summary>
public class UserInfo : AggregateRoot
{
    public string FirebaseUid { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Username { get; set; }
    public string? DisplayName { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public string? Theme { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? LastLoginAt { get; set; }
}
