using Lifenote.API.Mappings;
using Lifenote.API.Models.Requests.Note;
using Lifenote.API.Models.Responses;
using Lifenote.Application.Contracts;
using Lifenote.Application.DTOs.Note;
using Microsoft.AspNetCore.Mvc;

namespace Lifenote.API.Controllers;

/// <summary>
/// Notes controller — Phase 3.
/// Extends ApiControllerBase: [Authorize] and GetUserIdAsync() inherited.
/// No try/catch — ExceptionHandlingMiddleware handles all exceptions globally.
/// </summary>
[Route("api/[controller]")]
public class NoteController : ApiControllerBase
{
    private readonly INoteService _noteService;

    public NoteController(INoteService noteService, ICurrentUserService currentUserService)
        : base(currentUserService)
    {
        _noteService = noteService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<NoteDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<NoteDto>>>> GetNotes()
    {
        var userId = await GetUserIdAsync();
        var notes = await _noteService.GetAllNotesAsync(userId);
        return Ok(ApiResponse<IEnumerable<NoteDto>>.Success(notes));
    }

    [HttpGet("pinned")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<NoteDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<NoteDto>>>> GetPinnedNotes()
    {
        var userId = await GetUserIdAsync();
        var notes = await _noteService.GetPinnedNotesAsync(userId);
        return Ok(ApiResponse<IEnumerable<NoteDto>>.Success(notes));
    }

    [HttpGet("search")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<NoteDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<NoteDto>>>> SearchNotes(
        [FromQuery] string q)
    {
        var userId = await GetUserIdAsync();
        var notes = await _noteService.SearchNotesAsync(userId, q);
        return Ok(ApiResponse<IEnumerable<NoteDto>>.Success(notes));
    }

    [HttpGet("category/{category}")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<NoteDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<NoteDto>>>> GetNotesByCategory(
        string category)
    {
        var userId = await GetUserIdAsync();
        var notes = await _noteService.GetNotesByCategoryAsync(userId, category);
        return Ok(ApiResponse<IEnumerable<NoteDto>>.Success(notes));
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<NoteDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<NoteDto>>> GetNote(Guid id)
    {
        var userId = await GetUserIdAsync();
        var note = await _noteService.GetNoteByIdAsync(id, userId);
        if (note == null)
            return NotFound(ApiResponse<NoteDto>.Fail("Note not found."));
        return Ok(ApiResponse<NoteDto>.Success(note));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<NoteDto>), StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<NoteDto>>> CreateNote(
        [FromBody] CreateNoteRequest request)
    {
        var userId = await GetUserIdAsync();
        var created = await _noteService.CreateNoteAsync(userId, request.ToDto());
        return CreatedAtAction(
            nameof(GetNote),
            new { id = created.Id },
            ApiResponse<NoteDto>.Success(created, "Note created successfully."));
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<NoteDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<NoteDto>>> UpdateNote(
        Guid id, [FromBody] UpdateNoteRequest request)
    {
        var userId = await GetUserIdAsync();
        var updated = await _noteService.UpdateNoteAsync(id, userId, request.ToDto());
        return Ok(ApiResponse<NoteDto>.Success(updated));
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object>>> DeleteNote(Guid id)
    {
        var userId = await GetUserIdAsync();
        var result = await _noteService.DeleteNoteAsync(id, userId);
        if (!result)
            return NotFound(ApiResponse<object>.Fail("Note not found."));
        return Ok(ApiResponse<object>.Success(new { }, "Note deleted successfully."));
    }

    [HttpPatch("{id}/pin")]
    [ProducesResponseType(typeof(ApiResponse<NoteDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<NoteDto>>> TogglePin(Guid id)
    {
        var userId = await GetUserIdAsync();
        var note = await _noteService.TogglePinNoteAsync(id, userId);
        return Ok(ApiResponse<NoteDto>.Success(note));
    }

    [HttpPatch("{id}/archive")]
    [ProducesResponseType(typeof(ApiResponse<NoteDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<NoteDto>>> ToggleArchive(Guid id)
    {
        var userId = await GetUserIdAsync();
        var note = await _noteService.ToggleArchiveNoteAsync(id, userId);
        return Ok(ApiResponse<NoteDto>.Success(note));
    }
}
