using Lifenote.Domain.Entities;

namespace Lifenote.Domain.Interfaces;

/// <summary>
/// Repository interface for Note aggregate.
/// Defined in Domain — Infrastructure provides the EF Core implementation.
/// </summary>
public interface INoteRepository
{
    Task<IEnumerable<Note>> GetAllAsync(int userId);
    Task<IEnumerable<Note>> GetPinnedAsync(int userId);
    Task<IEnumerable<Note>> SearchAsync(int userId, string searchTerm);
    Task<IEnumerable<Note>> GetByCategoryAsync(int userId, string category);
    Task<Note?> GetByIdAsync(int id, int userId);
    Task<Note> CreateAsync(Note note);
    Task<Note> UpdateAsync(Note note);
    Task<bool> DeleteAsync(int id, int userId);
}
