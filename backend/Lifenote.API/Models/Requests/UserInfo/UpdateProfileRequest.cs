using System.ComponentModel.DataAnnotations;

namespace Lifenote.API.Models.Requests.UserInfo;

/// <summary>
/// HTTP request body for PUT /api/userinfo/me.
/// </summary>
public class UpdateProfileRequest
{
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    public DateTime? DateOfBirth { get; set; }

    [MaxLength(500)]
    public string? Bio { get; set; }
}
