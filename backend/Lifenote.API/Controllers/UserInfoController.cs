using Lifenote.API.Mappings;
using Lifenote.API.Models.Requests.UserInfo;
using Lifenote.API.Models.Responses;
using Lifenote.Application.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lifenote.API.Controllers;

/// <summary>
/// UserInfo controller — Phase 3.
/// Extends ApiControllerBase: [Authorize], GetFirebaseUid(), GetEmailFromToken() inherited.
/// UpdateTheme now accepts UpdateThemeRequest (API model) instead of Application DTO.
/// No try/catch — ExceptionHandlingMiddleware handles all exceptions globally.
/// </summary>
[Route("api/[controller]")]
public class UserInfoController : ApiControllerBase
{
    private readonly IUserInfoService _userInfoService;
    private readonly IFirebaseClaimService _firebaseClaimService;

    public UserInfoController(
        IUserInfoService userService,
        IFirebaseClaimService firebaseClaimService,
        ICurrentUserService currentUserService)
        : base(currentUserService)
    {
        _userInfoService = userService;
        _firebaseClaimService = firebaseClaimService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<UserProfileResponse>), StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<UserProfileResponse>>> CreateUser(
        [FromBody] CreateUserRequest request)
    {
        var firebaseUid = GetFirebaseUid();
        var email = GetEmailFromToken()
            ?? throw new UnauthorizedAccessException("Email missing from token.");
        var user = await _userInfoService.CreateUserAsync(firebaseUid, email, request.ToDto());
        await _firebaseClaimService.SetAppUserIdClaimAsync(firebaseUid, user.Id);
        return CreatedAtAction(
            nameof(GetCurrentUser),
            new { id = user.Id },
            ApiResponse<UserProfileResponse>.Success(user.ToResponse(), "User created successfully."));
    }

    [HttpGet("me")]
    [ProducesResponseType(typeof(ApiResponse<UserProfileResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<UserProfileResponse>>> GetCurrentUser()
    {
        var firebaseUid = GetFirebaseUid();
        var user = await _userInfoService.GetUserByAuthIdAsync(firebaseUid);
        return Ok(ApiResponse<UserProfileResponse>.Success(user.ToResponse()));
    }

    [HttpPut("me")]
    [ProducesResponseType(typeof(ApiResponse<UserProfileResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<UserProfileResponse>>> UpdateProfile(
        [FromBody] UpdateProfileRequest request)
    {
        var firebaseUid = GetFirebaseUid();
        var user = await _userInfoService.UpdateProfileAsync(firebaseUid, request.ToDto());
        return Ok(ApiResponse<UserProfileResponse>.Success(user.ToResponse()));
    }

    [HttpPatch("me/theme")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<object>>> UpdateTheme(
        [FromBody] UpdateThemeRequest request)
    {
        var firebaseUid = GetFirebaseUid();
        await _userInfoService.UpdateThemeAsync(firebaseUid, request.ToDto().Theme);
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
