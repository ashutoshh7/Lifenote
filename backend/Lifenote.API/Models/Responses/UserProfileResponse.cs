namespace Lifenote.API.Models.Responses;

/// <summary>
/// API-layer HTTP response shape for user profile endpoints.
/// Mapped FROM Application.DTOs.UserInfo.UserProfileDto by the controller or a mapper.
/// This is what gets serialized to JSON — not the Application DTO.
/// </summary>
public class UserProfileResponse
{
    public int Id { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}".Trim();
    public string Email { get; init; } = string.Empty;
    public string Username { get; init; } = string.Empty;
    public DateTime? DateOfBirth { get; init; }
    public string? ProfilePicture { get; init; }
    public string? Bio { get; init; }
    public string Theme { get; init; } = "light";
    public DateTime? LastLoginAt { get; init; }
    public DateTime CreatedAt { get; init; }
}
