using Lifenote.Domain.Entities;

namespace Lifenote.Domain.Interfaces;

/// <summary>
/// Repository interface for Note aggregate.
/// Defined in Domain — Infrastructure provides the EF Core implementation.
/// </summary>
public interface INoteRepository
{
    // --- Queries ---
    Task<IEnumerable<Note>> GetAllAsync(Guid userId);
    Task<IEnumerable<Note>> GetPinnedAsync(Guid userId);
    Task<IEnumerable<Note>> SearchAsync(Guid userId, string searchTerm);
    Task<IEnumerable<Note>> GetByCategoryAsync(Guid userId, string category);

    /// <summary>Get a note by its id, scoped to the owning user.</summary>
    Task<Note?> GetByIdAsync(Guid id, Guid userId);

    /// <summary>Get a note by id only — used internally when userId check is done in service layer.</summary>
    Task<Note?> GetByIdAsync(Guid id);

    Task<bool> ExistsAsync(Guid id, Guid userId);

    // --- Write operations ---

    /// <summary>Insert a new Note (void — caller uses UoW.SaveChanges).</summary>
    Task AddAsync(Note note);

    /// <summary>Canonical async create that returns the persisted entity.</summary>
    Task<Note> CreateAsync(Note note);

    /// <summary>Mark entity as modified in the change tracker (sync, EF Core pattern).</summary>
    void Update(Note note);

    /// <summary>Async update that persists immediately and returns the updated entity.</summary>
    Task<Note> UpdateAsync(Note note);

    /// <summary>Delete by id only (ownership already verified by service before calling).</summary>
    Task RemoveAsync(Guid id);

    /// <summary>Delete scoped to user — returns true if deleted.</summary>
    Task<bool> DeleteAsync(Guid id, Guid userId);
}
