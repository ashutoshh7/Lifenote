using Lifenote.Application.Contracts;
using Lifenote.Application.DTOs.Note;
using Lifenote.Domain.Entities;

namespace Lifenote.Application.Services;

/// <summary>
/// Moved from Lifenote.Data/Services/NoteService.cs.
/// Service logic lives in Application — no direct EF Core usage.
/// </summary>
public class NoteService : INoteService
{
    private readonly IUnitOfWork _unitOfWork;

    public NoteService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<IEnumerable<NoteDto>> GetAllNotesAsync(int userId)
    {
        var notes = await _unitOfWork.Notes.GetAllAsync(userId);
        return notes.Select(MapToDto);
    }

    public async Task<IEnumerable<NoteDto>> GetPinnedNotesAsync(int userId)
    {
        var notes = await _unitOfWork.Notes.GetPinnedAsync(userId);
        return notes.Select(MapToDto);
    }

    public async Task<IEnumerable<NoteDto>> SearchNotesAsync(int userId, string searchTerm)
    {
        var notes = await _unitOfWork.Notes.SearchAsync(userId, searchTerm);
        return notes.Select(MapToDto);
    }

    public async Task<IEnumerable<NoteDto>> GetNotesByCategoryAsync(int userId, string category)
    {
        var notes = await _unitOfWork.Notes.GetByCategoryAsync(userId, category);
        return notes.Select(MapToDto);
    }

    public async Task<NoteDto?> GetNoteByIdAsync(int id, int userId)
    {
        var note = await _unitOfWork.Notes.GetByIdAsync(id, userId);
        return note == null ? null : MapToDto(note);
    }

    public async Task<NoteDto> CreateNoteAsync(int userId, CreateNoteDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Title))
            throw new ArgumentException("Note title is required");

        var note = new Note
        {
            UserId = userId,
            Title = dto.Title.Trim(),
            Content = dto.Content,
            Category = dto.Category,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Notes.CreateAsync(note);
        await _unitOfWork.SaveChangesAsync();
        return MapToDto(note);
    }

    public async Task<NoteDto> UpdateNoteAsync(int id, int userId, UpdateNoteDto dto)
    {
        var note = await _unitOfWork.Notes.GetByIdAsync(id, userId)
            ?? throw new UnauthorizedAccessException("Note not found or access denied");

        if (!string.IsNullOrWhiteSpace(dto.Title)) note.Title = dto.Title.Trim();
        if (dto.Content != null) note.Content = dto.Content;
        if (dto.Category != null) note.Category = dto.Category;
        note.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Notes.UpdateAsync(note);
        await _unitOfWork.SaveChangesAsync();
        return MapToDto(note);
    }

    public async Task<bool> DeleteNoteAsync(int id, int userId)
    {
        var result = await _unitOfWork.Notes.DeleteAsync(id, userId);
        if (result) await _unitOfWork.SaveChangesAsync();
        return result;
    }

    public async Task<NoteDto> TogglePinNoteAsync(int id, int userId)
    {
        var note = await _unitOfWork.Notes.GetByIdAsync(id, userId)
            ?? throw new ArgumentException("Note not found");
        note.IsPinned = !note.IsPinned;
        note.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.Notes.UpdateAsync(note);
        await _unitOfWork.SaveChangesAsync();
        return MapToDto(note);
    }

    public async Task<NoteDto> ToggleArchiveNoteAsync(int id, int userId)
    {
        var note = await _unitOfWork.Notes.GetByIdAsync(id, userId)
            ?? throw new ArgumentException("Note not found");
        note.IsArchived = !note.IsArchived;
        note.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.Notes.UpdateAsync(note);
        await _unitOfWork.SaveChangesAsync();
        return MapToDto(note);
    }

    private static NoteDto MapToDto(Note note) => new()
    {
        Id = note.Id,
        Title = note.Title,
        Content = note.Content,
        Category = note.Category,
        IsPinned = note.IsPinned,
        IsArchived = note.IsArchived,
        CreatedAt = note.CreatedAt,
        UpdatedAt = note.UpdatedAt
    };
}
