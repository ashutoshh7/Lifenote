using Lifenote.Application.DTOs.Note;

namespace Lifenote.Application.Interfaces;

/// <summary>
/// Application service contract for Note use-cases.
/// Moved from Lifenote.Core.Interfaces — canonical home is now Application layer.
/// </summary>
public interface INoteService
{
    Task<IEnumerable<NoteDto>> GetAllNotesAsync(int userId);
    Task<NoteDto?> GetNoteByIdAsync(int id, int userId);
    Task<NoteDto> CreateNoteAsync(int userId, CreateNoteDto dto);
    Task<NoteDto> UpdateNoteAsync(int id, int userId, UpdateNoteDto dto);
    Task<bool> DeleteNoteAsync(int id, int userId);
    Task<IEnumerable<NoteDto>> GetNotesByCategoryAsync(int userId, string category);
    Task<IEnumerable<NoteDto>> GetPinnedNotesAsync(int userId);
    Task<IEnumerable<NoteDto>> SearchNotesAsync(int userId, string searchTerm);
    Task<NoteDto> TogglePinNoteAsync(int id, int userId);
    Task<NoteDto> ToggleArchiveNoteAsync(int id, int userId);
}
