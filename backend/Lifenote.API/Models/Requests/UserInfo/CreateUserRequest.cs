using System.ComponentModel.DataAnnotations;

namespace Lifenote.API.Models.Requests.UserInfo;

/// <summary>
/// HTTP request body for POST /api/users (register/provision after Firebase auth).
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
    [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Username can only contain letters, numbers, and underscores.")]
    public string Username { get; set; } = string.Empty;

    public DateTime? DateOfBirth { get; set; }
}
