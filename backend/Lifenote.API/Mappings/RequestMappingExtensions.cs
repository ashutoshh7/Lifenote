using Lifenote.API.Models.Requests.Habit;
using Lifenote.API.Models.Requests.Note;
using Lifenote.API.Models.Requests.UserInfo;
using Lifenote.Application.DTOs.Habit;
using Lifenote.Application.DTOs.Note;
using Lifenote.Application.DTOs.UserInfo;

namespace Lifenote.API.Mappings;

/// <summary>
/// Static extension methods that map API request models → Application DTOs.
/// Keeps controllers thin and avoids an AutoMapper dependency.
/// Rule: only this file is allowed to reference both API.Models.Requests and Application.DTOs.
/// </summary>
public static class RequestMappingExtensions
{
    // ── Habit ─────────────────────────────────────────────────────────────────

    public static CreateHabitDto ToDto(this CreateHabitRequest req) => new()
    {
        Name          = req.Name,
        Description   = req.Description,
        Color         = req.Color,
        IconName      = req.IconName,
        FrequencyType = req.FrequencyType,
        FrequencyValue = req.FrequencyValue,
        TargetCount   = req.TargetCount,
        StartDate     = req.StartDate,
        EndDate       = req.EndDate
    };

    public static UpdateHabitDto ToDto(this UpdateHabitRequest req) => new()
    {
        Name          = req.Name,
        Description   = req.Description,
        Color         = req.Color,
        IconName      = req.IconName,
        FrequencyType = req.FrequencyType,
        FrequencyValue = req.FrequencyValue,
        TargetCount   = req.TargetCount,
        IsActive      = req.IsActive,
        EndDate       = req.EndDate
    };

    public static CheckInDto ToDto(this CheckInRequest req) => new()
    {
        HabitId     = req.HabitId,
        Notes       = req.Notes,
        CompletedAt = req.CompletedAt
    };

    // ── Note ──────────────────────────────────────────────────────────────────

    public static CreateNoteDto ToDto(this CreateNoteRequest req) => new()
    {
        Title     = req.Title,
        Content   = req.Content,
        Category  = req.Category,
        IsPinned  = req.IsPinned
    };

    public static UpdateNoteDto ToDto(this UpdateNoteRequest req) => new()
    {
        Title      = req.Title,
        Content    = req.Content,
        Category   = req.Category,
        IsPinned   = req.IsPinned,
        IsArchived = req.IsArchived
    };

    // ── UserInfo ──────────────────────────────────────────────────────────────

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
}
