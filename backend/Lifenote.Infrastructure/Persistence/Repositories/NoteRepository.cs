using Lifenote.Core.Interfaces;
using Lifenote.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Lifenote.Infrastructure.Persistence.Repositories
{
    public class NoteRepository : INoteRepository
    {
        private readonly LifenoteDbContext _context;

        public NoteRepository(LifenoteDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Note>> GetAllAsync(int userId)
            => await _context.Notes
                .Where(n => n.UserId == userId && n.IsArchived == false)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

        public async Task<Note?> GetByIdAsync(int id)
            => await _context.Notes.FindAsync(id);

        public async Task<Note> CreateAsync(Note note)
        {
            note.CreatedAt = DateTime.UtcNow;
            note.UpdatedAt = DateTime.UtcNow;
            _context.Notes.Add(note);
            return note;
        }

        public async Task<Note> UpdateAsync(Note note)
        {
            note.UpdatedAt = DateTime.UtcNow;
            _context.Entry(note).State = EntityState.Modified;
            return note;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var note = await _context.Notes.FindAsync(id);
            if (note == null) return false;
            _context.Notes.Remove(note);
            return true;
        }

        public async Task<IEnumerable<Note>> GetByCategoryAsync(int userId, string category)
            => await _context.Notes
                .Where(n => n.UserId == userId && n.Category == category && n.IsArchived == false)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

        public async Task<IEnumerable<Note>> GetPinnedAsync(int userId)
            => await _context.Notes
                .Where(n => n.UserId == userId && n.IsPinned == true && n.IsArchived == false)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

        public async Task<IEnumerable<Note>> SearchAsync(int userId, string searchTerm)
            => await _context.Notes
                .Where(n => n.UserId == userId && n.IsArchived == false &&
                            (n.Title.Contains(searchTerm) || n.Content.Contains(searchTerm)))
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

        public async Task<bool> ExistsAsync(int id, int userId)
            => await _context.Notes.AnyAsync(n => n.Id == id && n.UserId == userId);

        public async Task AddAsync(Note note)
        {
            note.CreatedAt = DateTime.UtcNow;
            note.UpdatedAt = DateTime.UtcNow;
            await _context.Notes.AddAsync(note);
        }

        public void Update(Note note)
        {
            note.UpdatedAt = DateTime.UtcNow;
            _context.Entry(note).State = EntityState.Modified;
        }

        public async Task RemoveAsync(int id)
        {
            var note = await _context.Notes.FindAsync(id);
            if (note != null) _context.Notes.Remove(note);
        }
    }
}
