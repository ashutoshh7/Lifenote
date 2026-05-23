using Lifenote.Domain.Entities;
using Lifenote.Domain.Interfaces;
using Lifenote.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Lifenote.Infrastructure.Repositories;

public class NoteRepository : INoteRepository
{
    private readonly LifenoteDbContext _db;

    public NoteRepository(LifenoteDbContext db) => _db = db;

    public async Task<IEnumerable<Note>> GetAllAsync(int userId) =>
        await _db.Notes
            .Where(n => n.UserId == userId && !n.IsArchived)
            .AsNoTracking()
            .OrderByDescending(n => n.IsPinned)
            .ThenByDescending(n => n.UpdatedAt)
            .ToListAsync();

    public async Task<IEnumerable<Note>> GetPinnedAsync(int userId) =>
        await _db.Notes
            .Where(n => n.UserId == userId && n.IsPinned && !n.IsArchived)
            .AsNoTracking()
            .ToListAsync();

    public async Task<IEnumerable<Note>> SearchAsync(int userId, string searchTerm) =>
        await _db.Notes
            .Where(n => n.UserId == userId &&
                (n.Title.Contains(searchTerm) || (n.Content != null && n.Content.Contains(searchTerm))))
            .AsNoTracking()
            .ToListAsync();

    public async Task<IEnumerable<Note>> GetByCategoryAsync(int userId, string category) =>
        await _db.Notes
            .Where(n => n.UserId == userId && n.Category == category)
            .AsNoTracking()
            .ToListAsync();

    // Scoped to user — canonical query
    public Task<Note?> GetByIdAsync(int id, int userId) =>
        _db.Notes.FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);

    // Unscoped — used when service already verified ownership
    public Task<Note?> GetByIdAsync(int id) =>
        _db.Notes.FirstOrDefaultAsync(n => n.Id == id);

    public Task<bool> ExistsAsync(int id, int userId) =>
        _db.Notes.AnyAsync(n => n.Id == id && n.UserId == userId);

    public async Task AddAsync(Note note) =>
        await _db.Notes.AddAsync(note);

    public async Task<Note> CreateAsync(Note note)
    {
        await _db.Notes.AddAsync(note);
        return note;
    }

    public void Update(Note note) =>
        _db.Notes.Update(note);

    public Task<Note> UpdateAsync(Note note)
    {
        _db.Notes.Update(note);
        return Task.FromResult(note);
    }

    public async Task RemoveAsync(int id)
    {
        var note = await _db.Notes.FindAsync(id);
        if (note != null) _db.Notes.Remove(note);
    }

    public async Task<bool> DeleteAsync(int id, int userId)
    {
        var note = await _db.Notes.FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);
        if (note == null) return false;
        _db.Notes.Remove(note);
        return true;
    }
}
