using Lifenote.Application.DTOs.Note;

namespace Lifenote.Application.Contracts;

public interface INoteService
{
    Task<IEnumerable<NoteDto>> GetAllNotesAsync(Guid userId);
    Task<NoteDto?> GetNoteByIdAsync(Guid id, Guid userId);
    Task<NoteDto> CreateNoteAsync(Guid userId, CreateNoteDto dto);
    Task<NoteDto> UpdateNoteAsync(Guid id, Guid userId, UpdateNoteDto dto);
    Task<bool> DeleteNoteAsync(Guid id, Guid userId);
    Task<IEnumerable<NoteDto>> GetNotesByCategoryAsync(Guid userId, string category);
    Task<IEnumerable<NoteDto>> GetPinnedNotesAsync(Guid userId);
    Task<IEnumerable<NoteDto>> SearchNotesAsync(Guid userId, string searchTerm);
    Task<NoteDto> TogglePinNoteAsync(Guid id, Guid userId);
    Task<NoteDto> ToggleArchiveNoteAsync(Guid id, Guid userId);
}
