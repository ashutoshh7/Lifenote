using Lifenote.Application.Contracts;
using Lifenote.Application.DTOs.Note;
using Lifenote.Domain.Entities;
using Lifenote.Domain.Exceptions;
using Lifenote.Domain.Interfaces;

namespace Lifenote.Application.Services
{
    public class NoteService : INoteService
    {
        private readonly IUnitOfWork _unitOfWork;

        public NoteService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // ---------------------------------------------------------------
        // Tags conversion helpers
        // Entity stores Tags as a comma-separated string? (EF-friendly).
        // DTOs expose Tags as List<string>? (API/JSON-friendly).
        // ---------------------------------------------------------------
        private static string? TagsToString(List<string>? tags)
            => tags == null || tags.Count == 0 ? null : string.Join(',', tags);

        private static List<string>? StringToTags(string? raw)
            => string.IsNullOrWhiteSpace(raw)
                ? null
                : raw.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                     .ToList();

        // ---------------------------------------------------------------

        public async Task<IEnumerable<NoteDto>> GetAllNotesAsync(int userId)
        {
            var notes = await _unitOfWork.Notes.GetAllAsync(userId);
            return notes.Select(MapToDto);
        }

        public async Task<NoteDto?> GetNoteByIdAsync(int id, int userId)
        {
            var note = await _unitOfWork.Notes.GetByIdAsync(id, userId);
            if (note == null) return null;
            return MapToDto(note);
        }

        public async Task<NoteDto> CreateNoteAsync(int userId, CreateNoteDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Title))
                throw new DomainException("Note title cannot be empty.");
            if (string.IsNullOrWhiteSpace(dto.Content))
                throw new DomainException("Note content cannot be empty.");

            var note = new Note
            {
                UserId     = userId,
                Title      = dto.Title,
                Content    = dto.Content,
                Category   = dto.Category,
                Tags       = TagsToString(dto.Tags),
                IsPinned   = dto.IsPinned,
                IsArchived = false
            };
            await _unitOfWork.Notes.AddAsync(note);
            await _unitOfWork.SaveChangesAsync();
            return MapToDto(note);
        }

        public async Task<NoteDto> UpdateNoteAsync(int id, int userId, UpdateNoteDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Title))
                throw new DomainException("Note title cannot be empty.");
            if (string.IsNullOrWhiteSpace(dto.Content))
                throw new DomainException("Note content cannot be empty.");

            var existing = await _unitOfWork.Notes.GetByIdAsync(id, userId)
                ?? throw new NotFoundException($"Note {id} not found or access denied.");

            existing.Title      = dto.Title;
            existing.Content    = dto.Content;
            existing.Category   = dto.Category;
            existing.Tags       = TagsToString(dto.Tags);
            existing.IsPinned   = dto.IsPinned;
            existing.IsArchived = dto.IsArchived;
            existing.UpdatedAt  = DateTime.UtcNow;

            _unitOfWork.Notes.Update(existing);
            await _unitOfWork.SaveChangesAsync();
            return MapToDto(existing);
        }

        public async Task<bool> DeleteNoteAsync(int id, int userId)
        {
            if (!await _unitOfWork.Notes.ExistsAsync(id, userId)) return false;
            await _unitOfWork.Notes.RemoveAsync(id);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<NoteDto>> GetNotesByCategoryAsync(int userId, string category)
        {
            var notes = await _unitOfWork.Notes.GetByCategoryAsync(userId, category);
            return notes.Select(MapToDto);
        }

        public async Task<IEnumerable<NoteDto>> GetPinnedNotesAsync(int userId)
        {
            var notes = await _unitOfWork.Notes.GetPinnedAsync(userId);
            return notes.Select(MapToDto);
        }

        public async Task<IEnumerable<NoteDto>> SearchNotesAsync(int userId, string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm)) return await GetAllNotesAsync(userId);
            var notes = await _unitOfWork.Notes.SearchAsync(userId, searchTerm);
            return notes.Select(MapToDto);
        }

        public async Task<NoteDto> TogglePinNoteAsync(int id, int userId)
        {
            var note = await _unitOfWork.Notes.GetByIdAsync(id, userId)
                ?? throw new NotFoundException($"Note {id} not found.");
            note.IsPinned  = !note.IsPinned;
            note.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Notes.Update(note);
            await _unitOfWork.SaveChangesAsync();
            return MapToDto(note);
        }

        public async Task<NoteDto> ToggleArchiveNoteAsync(int id, int userId)
        {
            var note = await _unitOfWork.Notes.GetByIdAsync(id, userId)
                ?? throw new NotFoundException($"Note {id} not found.");
            note.IsArchived = !note.IsArchived;
            note.UpdatedAt  = DateTime.UtcNow;
            _unitOfWork.Notes.Update(note);
            await _unitOfWork.SaveChangesAsync();
            return MapToDto(note);
        }

        /// <summary>
        /// Maps Note entity → NoteDto.
        /// note.Content is string? on the entity; NoteDto.Content is string.
        /// Coalesce with string.Empty to avoid CS8601.
        /// </summary>
        private static NoteDto MapToDto(Note note) => new NoteDto
        {
            Id         = note.Id,
            UserId     = note.UserId,
            Title      = note.Title,
            Content    = note.Content    ?? string.Empty,
            Category   = note.Category,
            Tags       = StringToTags(note.Tags),
            IsPinned   = note.IsPinned,
            IsArchived = note.IsArchived,
            CreatedAt  = note.CreatedAt,
            UpdatedAt  = note.UpdatedAt
        };
    }
}
