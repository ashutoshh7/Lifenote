using Lifenote.Application.Contracts;
using Lifenote.Application.DTOs.UserInfo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Lifenote.API.Controllers;

/// <summary>
/// Refactored: IFirebaseClaimService now injected from Application.Contracts
/// (interface moved out of API project). DTOs from Application.DTOs.UserInfo.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserInfoController : ControllerBase
{
    private readonly IUserInfoService _userInfoService;
    private readonly IFirebaseClaimService _firebaseClaimService;

    public UserInfoController(IUserInfoService userService, IFirebaseClaimService firebaseClaimService)
    {
        _userInfoService = userService;
        _firebaseClaimService = firebaseClaimService;
    }

    private string GetFirebaseUid() =>
        User.FindFirst("user_id")?.Value
        ?? throw new UnauthorizedAccessException("Invalid token");

    private string? GetEmailFromToken() =>
        User.FindFirst(ClaimTypes.Email)?.Value;

    [HttpPost]
    public async Task<ActionResult<UserProfileResponse>> CreateUser([FromBody] CreateUserDto request)
    {
        var firebaseUid = GetFirebaseUid();
        var email = GetEmailFromToken() ?? throw new UnauthorizedAccessException("Email missing from token");
        var user = await _userInfoService.CreateUserAsync(firebaseUid, email, request);
        await _firebaseClaimService.SetAppUserIdClaimAsync(firebaseUid, user.Id);
        return CreatedAtAction(nameof(GetCurrentUser), new { id = user.Id }, user);
    }

    [HttpGet("me")]
    public async Task<ActionResult<UserProfileResponse>> GetCurrentUser()
    {
        var firebaseUid = GetFirebaseUid();
        var user = await _userInfoService.GetUserByAuthIdAsync(firebaseUid);
        return Ok(user);
    }

    [HttpPut("me")]
    public async Task<ActionResult<UserProfileResponse>> UpdateProfile([FromBody] UpdateProfileDto request)
    {
        var firebaseUid = GetFirebaseUid();
        var user = await _userInfoService.UpdateProfileAsync(firebaseUid, request);
        return Ok(user);
    }

    [HttpPatch("me/theme")]
    public async Task<IActionResult> UpdateTheme([FromBody] UpdateThemeDto request)
    {
        var firebaseUid = GetFirebaseUid();
        await _userInfoService.UpdateThemeAsync(firebaseUid, request.Theme);
        return NoContent();
    }

    [HttpPatch("me/profile-picture")]
    public async Task<IActionResult> UpdateProfilePicture([FromBody] string profilePictureUrl)
    {
        var firebaseUid = GetFirebaseUid();
        await _userInfoService.UpdateProfilePictureAsync(firebaseUid, profilePictureUrl);
        return NoContent();
    }

    [HttpPatch("me/last-login")]
    public async Task<IActionResult> UpdateLastLogin()
    {
        var firebaseUid = GetFirebaseUid();
        await _userInfoService.UpdateLastLoginAsync(firebaseUid);
        return NoContent();
    }

    [HttpDelete("me")]
    public async Task<IActionResult> DeactivateAccount()
    {
        var firebaseUid = GetFirebaseUid();
        await _userInfoService.DeactivateUserAsync(firebaseUid);
        return NoContent();
    }

    [HttpGet("check-username/{username}")]
    [AllowAnonymous]
    public async Task<ActionResult<object>> CheckUsername(string username)
    {
        var isAvailable = await _userInfoService.IsUsernameAvailableAsync(username);
        return Ok(new { available = isAvailable });
    }
}
