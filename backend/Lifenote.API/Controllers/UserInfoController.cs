using Lifenote.API.Mappings;
using Lifenote.API.Models.Requests.UserInfo;
using Lifenote.API.Models.Responses;
using Lifenote.Application.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UpdateThemeDto = Lifenote.Application.DTOs.UserInfo.UpdateThemeDto;

namespace Lifenote.API.Controllers;

/// <summary>
/// UserInfo controller — Phase 2 Step 3.
/// - Accepts CreateUserRequest / UpdateProfileRequest (HTTP contract).
/// - Returns ApiResponse<T> or ApiResponse<UserProfileResponse> envelope.
/// - Maps request → DTO via RequestMappingExtensions.ToDto().
/// - No try/catch — ExceptionHandlingMiddleware handles globally.
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
    [ProducesResponseType(typeof(ApiResponse<UserProfileResponse>), StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<UserProfileResponse>>> CreateUser(
        [FromBody] CreateUserRequest request)
    {
        var firebaseUid = GetFirebaseUid();
        var email = GetEmailFromToken()
            ?? throw new UnauthorizedAccessException("Email missing from token");
        var user = await _userInfoService.CreateUserAsync(firebaseUid, email, request.ToDto());
        await _firebaseClaimService.SetAppUserIdClaimAsync(firebaseUid, user.Id);
        return CreatedAtAction(
            nameof(GetCurrentUser),
            new { id = user.Id },
            ApiResponse<UserProfileResponse>.Success(user, "User created successfully."));
    }

    [HttpGet("me")]
    [ProducesResponseType(typeof(ApiResponse<UserProfileResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<UserProfileResponse>>> GetCurrentUser()
    {
        var firebaseUid = GetFirebaseUid();
        var user = await _userInfoService.GetUserByAuthIdAsync(firebaseUid);
        return Ok(ApiResponse<UserProfileResponse>.Success(user));
    }

    [HttpPut("me")]
    [ProducesResponseType(typeof(ApiResponse<UserProfileResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<UserProfileResponse>>> UpdateProfile(
        [FromBody] UpdateProfileRequest request)
    {
        var firebaseUid = GetFirebaseUid();
        var user = await _userInfoService.UpdateProfileAsync(firebaseUid, request.ToDto());
        return Ok(ApiResponse<UserProfileResponse>.Success(user));
    }

    [HttpPatch("me/theme")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<object>>> UpdateTheme(
        [FromBody] UpdateThemeDto request)
    {
        var firebaseUid = GetFirebaseUid();
        await _userInfoService.UpdateThemeAsync(firebaseUid, request.Theme);
        return Ok(ApiResponse<object>.Success(new { }, "Theme updated successfully."));
    }

    [HttpPatch("me/profile-picture")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<object>>> UpdateProfilePicture(
        [FromBody] string profilePictureUrl)
    {
        var firebaseUid = GetFirebaseUid();
        await _userInfoService.UpdateProfilePictureAsync(firebaseUid, profilePictureUrl);
        return Ok(ApiResponse<object>.Success(new { }, "Profile picture updated."));
    }

    [HttpPatch("me/last-login")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<object>>> UpdateLastLogin()
    {
        var firebaseUid = GetFirebaseUid();
        await _userInfoService.UpdateLastLoginAsync(firebaseUid);
        return Ok(ApiResponse<object>.Success(new { }, "Last login updated."));
    }

    [HttpDelete("me")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<object>>> DeactivateAccount()
    {
        var firebaseUid = GetFirebaseUid();
        await _userInfoService.DeactivateUserAsync(firebaseUid);
        return Ok(ApiResponse<object>.Success(new { }, "Account deactivated."));
    }

    [HttpGet("check-username/{username}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<object>>> CheckUsername(string username)
    {
        var isAvailable = await _userInfoService.IsUsernameAvailableAsync(username);
        return Ok(ApiResponse<object>.Success(new { available = isAvailable }));
    }
}
