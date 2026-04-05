namespace Lifenote.Application.DTOs.UserInfo;

public class CreateUserDto
{
    public string? Username { get; set; }
    public string? DisplayName { get; set; }
}

public class UpdateProfileDto
{
    public string? Username { get; set; }
    public string? DisplayName { get; set; }
}

public class UpdateThemeDto
{
    public string Theme { get; set; } = string.Empty;
}

public class UserProfileResponse
{
    public int Id { get; set; }
    public string FirebaseUid { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Username { get; set; }
    public string? DisplayName { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public string? Theme { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
}
