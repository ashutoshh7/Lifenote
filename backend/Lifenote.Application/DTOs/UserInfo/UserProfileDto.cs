namespace Lifenote.Application.DTOs.UserInfo;

/// <summary>
/// Application-layer DTO representing a resolved user profile.
/// Renamed from UserProfileResponse — 'Response' suffix belongs in the API layer.
/// The API layer maps this into Lifenote.API.Models.Responses.UserProfileResponse.
/// </summary>
public class UserProfileDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }
    public string? ProfilePicture { get; set; }
    public string? Bio { get; set; }
    public string Theme { get; set; } = "light";
    public DateTime? LastLoginAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
