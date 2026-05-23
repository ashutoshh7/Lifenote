using System.ComponentModel.DataAnnotations;

namespace Lifenote.API.Models.Requests.UserInfo;

/// <summary>
/// HTTP request body for POST /api/userinfo.
/// Firebase UID and email are taken from the JWT — not accepted from the body.
/// </summary>
public class CreateUserRequest
{
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    [RegularExpression(@"^[a-zA-Z0-9_]{3,50}$",
        ErrorMessage = "Username must be 3-50 characters and contain only letters, digits, or underscores.")]
    public string Username { get; set; } = string.Empty;

    public DateTime? DateOfBirth { get; set; }
}
