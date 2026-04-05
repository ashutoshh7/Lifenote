namespace Lifenote.Application.DTOs.UserInfo;

/// <summary>
/// DTO used by Application services to create a user.
/// Moved from Lifenote.Core.DTOs — canonical home is now Application layer.
/// </summary>
public class CreateUserDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }
}

public class UpdateProfileDto
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Bio { get; set; }
}

public class UpdateThemeDto
{
    public string Theme { get; set; } = "light"; // "light" | "dark" | "auto"
}
