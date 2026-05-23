using System.ComponentModel.DataAnnotations;

namespace Lifenote.API.Models.Requests.UserInfo;

/// <summary>
/// HTTP request body for PATCH /api/userinfo/me/theme.
/// Replaces the direct use of Application.DTOs.UserInfo.UpdateThemeDto in the controller,
/// keeping Application DTOs strictly internal to the Application layer.
/// </summary>
public class UpdateThemeRequest
{
    [Required]
    [MaxLength(50)]
    public string Theme { get; set; } = string.Empty;
}
