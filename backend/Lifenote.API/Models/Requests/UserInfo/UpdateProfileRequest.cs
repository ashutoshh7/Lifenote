using System.ComponentModel.DataAnnotations;

namespace Lifenote.API.Models.Requests.UserInfo;

/// <summary>
/// HTTP request body for PATCH /api/users/profile.
/// </summary>
public class UpdateProfileRequest
{
    [MaxLength(100)]
    public string? FirstName { get; set; }

    [MaxLength(100)]
    public string? LastName { get; set; }

    public DateTime? DateOfBirth { get; set; }

    [MaxLength(500)]
    public string? Bio { get; set; }
}
