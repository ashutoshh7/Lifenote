using Lifenote.Application.DTOs.Note;

namespace Lifenote.Application.Contracts;

public interface INoteService
{
    Task<IEnumerable<NoteDto>> GetAllNotesAsync(int userId);
    Task<IEnumerable<NoteDto>> GetPinnedNotesAsync(int userId);
    Task<IEnumerable<NoteDto>> SearchNotesAsync(int userId, string searchTerm);
    Task<IEnumerable<NoteDto>> GetNotesByCategoryAsync(int userId, string category);
    Task<NoteDto?> GetNoteByIdAsync(int id, int userId);
    Task<NoteDto> CreateNoteAsync(int userId, CreateNoteDto dto);
    Task<NoteDto> UpdateNoteAsync(int id, int userId, UpdateNoteDto dto);
    Task<bool> DeleteNoteAsync(int id, int userId);
    Task<NoteDto> TogglePinNoteAsync(int id, int userId);
    Task<NoteDto> ToggleArchiveNoteAsync(int id, int userId);
}
