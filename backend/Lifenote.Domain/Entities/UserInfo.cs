namespace Lifenote.Domain.Entities;

public class UserInfo
{
    public int Id { get; set; }
    public string FirebaseUid { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Username { get; set; }
    public string? DisplayName { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public string? Theme { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
