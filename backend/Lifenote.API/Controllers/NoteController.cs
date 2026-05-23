using Lifenote.API.Mappings;
using Lifenote.API.Models.Requests.Note;
using Lifenote.API.Models.Responses;
using Lifenote.Application.Contracts;
using Lifenote.Application.DTOs.Note;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lifenote.API.Controllers;

/// <summary>
/// Notes controller — Phase 2 Step 3.
/// - Accepts CreateNoteRequest / UpdateNoteRequest (HTTP contract).
/// - Returns ApiResponse<T> envelope for every endpoint.
/// - Maps request → DTO via RequestMappingExtensions.ToDto().
/// - No try/catch — ExceptionHandlingMiddleware handles globally.
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class NoteController : ControllerBase
{
    private readonly INoteService _noteService;
    private readonly ICurrentUserService _currentUserService;

    public NoteController(INoteService noteService, ICurrentUserService currentUserService)
    {
        _noteService = noteService;
        _currentUserService = currentUserService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<NoteDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<NoteDto>>>> GetNotes()
    {
        var userId = await _currentUserService.GetCurrentUserIdAsync();
        var notes = await _noteService.GetAllNotesAsync(userId);
        return Ok(ApiResponse<IEnumerable<NoteDto>>.Success(notes));
    }

    [HttpGet("pinned")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<NoteDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<NoteDto>>>> GetPinnedNotes()
    {
        var userId = await _currentUserService.GetCurrentUserIdAsync();
        var notes = await _noteService.GetPinnedNotesAsync(userId);
        return Ok(ApiResponse<IEnumerable<NoteDto>>.Success(notes));
    }

    [HttpGet("search")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<NoteDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<NoteDto>>>> SearchNotes(
        [FromQuery] string q)
    {
        var userId = await _currentUserService.GetCurrentUserIdAsync();
        var notes = await _noteService.SearchNotesAsync(userId, q);
        return Ok(ApiResponse<IEnumerable<NoteDto>>.Success(notes));
    }

    [HttpGet("category/{category}")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<NoteDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<NoteDto>>>> GetNotesByCategory(
        string category)
    {
        var userId = await _currentUserService.GetCurrentUserIdAsync();
        var notes = await _noteService.GetNotesByCategoryAsync(userId, category);
        return Ok(ApiResponse<IEnumerable<NoteDto>>.Success(notes));
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<NoteDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<NoteDto>>> GetNote(int id)
    {
        var userId = await _currentUserService.GetCurrentUserIdAsync();
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
        var userId = await _currentUserService.GetCurrentUserIdAsync();
        var created = await _noteService.CreateNoteAsync(userId, request.ToDto());
        return CreatedAtAction(
            nameof(GetNote),
            new { id = created.Id },
            ApiResponse<NoteDto>.Success(created, "Note created successfully."));
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<NoteDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<NoteDto>>> UpdateNote(
        int id, [FromBody] UpdateNoteRequest request)
    {
        var userId = await _currentUserService.GetCurrentUserIdAsync();
        var updated = await _noteService.UpdateNoteAsync(id, userId, request.ToDto());
        return Ok(ApiResponse<NoteDto>.Success(updated));
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object>>> DeleteNote(int id)
    {
        var userId = await _currentUserService.GetCurrentUserIdAsync();
        var result = await _noteService.DeleteNoteAsync(id, userId);
        if (!result)
            return NotFound(ApiResponse<object>.Fail("Note not found."));
        return Ok(ApiResponse<object>.Success(new { }, "Note deleted successfully."));
    }

    [HttpPatch("{id}/pin")]
    [ProducesResponseType(typeof(ApiResponse<NoteDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<NoteDto>>> TogglePin(int id)
    {
        var userId = await _currentUserService.GetCurrentUserIdAsync();
        var note = await _noteService.TogglePinNoteAsync(id, userId);
        return Ok(ApiResponse<NoteDto>.Success(note));
    }

    [HttpPatch("{id}/archive")]
    [ProducesResponseType(typeof(ApiResponse<NoteDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<NoteDto>>> ToggleArchive(int id)
    {
        var userId = await _currentUserService.GetCurrentUserIdAsync();
        var note = await _noteService.ToggleArchiveNoteAsync(id, userId);
        return Ok(ApiResponse<NoteDto>.Success(note));
    }
}
