using Lifenote.Application.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Lifenote.API.Controllers;

/// <summary>
/// Base controller for all Lifenote API controllers.
///
/// Responsibilities:
///   - Provides <see cref="GetUserIdAsync"/> so every action can resolve the
///     internal integer user-id without copy-pasting the same call.
///   - Provides <see cref="GetFirebaseUid"/> and <see cref="GetEmailFromToken"/>
///     for controllers that still need raw Firebase claims (e.g. UserInfoController).
///
/// Convention:
///   - Apply [Authorize] here so derived controllers inherit it by default.
///     Use [AllowAnonymous] on individual actions that must be public.
/// </summary>
[Authorize]
[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    protected readonly ICurrentUserService CurrentUserService;

    protected ApiControllerBase(ICurrentUserService currentUserService)
    {
        CurrentUserService = currentUserService;
    }

    /// <summary>
    /// Returns the internal integer UserId for the authenticated caller.
    /// </summary>
    protected Task<Guid> GetUserIdAsync()
        => CurrentUserService.GetCurrentUserIdAsync();

    /// <summary>
    /// Returns the Firebase UID from the JWT claim "user_id".
    /// Throws <see cref="UnauthorizedAccessException"/> if the claim is absent.
    /// </summary>
    protected string GetFirebaseUid()
        => User.FindFirst("user_id")?.Value
           ?? throw new UnauthorizedAccessException("Firebase UID missing from token.");

    /// <summary>
    /// Returns the email address from the JWT, or null if absent.
    /// </summary>
    protected string? GetEmailFromToken()
        => User.FindFirst(ClaimTypes.Email)?.Value;
}
