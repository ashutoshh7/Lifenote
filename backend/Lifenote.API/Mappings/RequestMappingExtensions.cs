using Lifenote.API.Models.Requests.Habit;
using Lifenote.API.Models.Requests.Note;
using Lifenote.API.Models.Requests.UserInfo;
using Lifenote.API.Models.Responses;
using Lifenote.Application.DTOs.Habit;
using Lifenote.Application.DTOs.Note;
using AppUserProfileResponse = Lifenote.Application.DTOs.UserInfo.UserProfileResponse;
using CreateUserDto          = Lifenote.Application.DTOs.UserInfo.CreateUserDto;
using UpdateProfileDto       = Lifenote.Application.DTOs.UserInfo.UpdateProfileDto;
using UpdateThemeDto         = Lifenote.Application.DTOs.UserInfo.UpdateThemeDto;

namespace Lifenote.API.Mappings;

/// <summary>
/// Static extension methods that map:
///   API request models  → Application DTOs       (ToDto)
///   Application DTOs    → API response models     (ToResponse)
///
/// Rule: only this file is allowed to reference both API.Models and Application.DTOs.
/// Unqualified 'UserProfileResponse' always refers to API.Models.Responses.UserProfileResponse.
/// </summary>
public static class RequestMappingExtensions
{
    // ── Habit ────────────────────────────────────────────────────────────────────

    public static CreateHabitDto ToDto(this CreateHabitRequest req) => new()
    {
        Name           = req.Name,
        Description    = req.Description,
        Color          = req.Color,
        IconName       = req.IconName,
        FrequencyType  = req.FrequencyType,
        FrequencyValue = req.FrequencyValue,
        TargetCount    = req.TargetCount,
        StartDate      = req.StartDate,
        EndDate        = req.EndDate
    };

    public static UpdateHabitDto ToDto(this UpdateHabitRequest req) => new()
    {
        Name           = req.Name,
        Description    = req.Description,
        Color          = req.Color,
        IconName       = req.IconName,
        FrequencyType  = req.FrequencyType,
        FrequencyValue = req.FrequencyValue,
        TargetCount    = req.TargetCount,
        IsActive       = req.IsActive,
        EndDate        = req.EndDate
    };

    /// <summary>
    /// CheckInDto has no CompletedAt — the service always uses DateTime.UtcNow.Date internally.
    /// CompletedAt on the request is intentionally dropped here.
    /// </summary>
    public static CheckInDto ToDto(this CheckInRequest req) => new()
    {
        HabitId = req.HabitId,
        Notes   = req.Notes
    };

    // ── Note ─────────────────────────────────────────────────────────────────────

    public static CreateNoteDto ToDto(this CreateNoteRequest req) => new()
    {
        Title    = req.Title,
        Content  = req.Content,
        Category = req.Category,
        IsPinned = req.IsPinned
    };

    /// <summary>
    /// UpdateNoteRequest uses bool? (nullable) for patch semantics.
    /// UpdateNoteDto uses bool (non-nullable). Use GetValueOrDefault() to convert safely.
    /// </summary>
    public static UpdateNoteDto ToDto(this UpdateNoteRequest req) => new()
    {
        Title      = req.Title      ?? string.Empty,
        Content    = req.Content    ?? string.Empty,
        Category   = req.Category,
        IsPinned   = req.IsPinned.GetValueOrDefault(),
        IsArchived = req.IsArchived.GetValueOrDefault()
    };

    // ── UserInfo ──────────────────────────────────────────────────────────────────

    public static CreateUserDto ToDto(this CreateUserRequest req) => new()
    {
        FirstName   = req.FirstName,
        LastName    = req.LastName,
        Username    = req.Username,
        DateOfBirth = req.DateOfBirth
    };

    public static UpdateProfileDto ToDto(this UpdateProfileRequest req) => new()
    {
        FirstName   = req.FirstName,
        LastName    = req.LastName,
        DateOfBirth = req.DateOfBirth,
        Bio         = req.Bio
    };

    /// <summary>
    /// Maps UpdateThemeRequest (API contract) → UpdateThemeDto (Application layer).
    /// Keeps Application DTOs out of controller method signatures.
    /// </summary>
    public static UpdateThemeDto ToDto(this UpdateThemeRequest req) => new()
    {
        Theme = req.Theme
    };

    /// <summary>
    /// Maps Application DTO → API response shape.
    /// 'AppUserProfileResponse' alias = Lifenote.Application.DTOs.UserInfo.UserProfileResponse
    /// Unqualified 'UserProfileResponse' = Lifenote.API.Models.Responses.UserProfileResponse
    /// </summary>
    public static UserProfileResponse ToResponse(this AppUserProfileResponse dto) =>
        new()
        {
            Id             = dto.Id,
            FirstName      = dto.FirstName,
            LastName       = dto.LastName,
            Email          = dto.Email,
            Username       = dto.Username,
            DateOfBirth    = dto.DateOfBirth,
            ProfilePicture = dto.ProfilePicture,
            Bio            = dto.Bio,
            Theme          = dto.Theme,
            LastLoginAt    = dto.LastLoginAt
        };
}
