using Lifenote.Application.Contracts;
using Lifenote.Application.DTOs.Note;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lifenote.API.Controllers;

/// <summary>
/// Refactored: namespace updated to Lifenote.Application.DTOs.Note.
/// All try/catch removed — ExceptionHandlingMiddleware handles globally.
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
    public async Task<ActionResult<IEnumerable<NoteDto>>> GetNotes()
    {
        var userId = await _currentUserService.GetCurrentUserIdAsync();
        return Ok(await _noteService.GetAllNotesAsync(userId));
    }

    [HttpGet("pinned")]
    public async Task<ActionResult<IEnumerable<NoteDto>>> GetPinnedNotes()
    {
        var userId = await _currentUserService.GetCurrentUserIdAsync();
        return Ok(await _noteService.GetPinnedNotesAsync(userId));
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<NoteDto>>> SearchNotes([FromQuery] string q)
    {
        var userId = await _currentUserService.GetCurrentUserIdAsync();
        return Ok(await _noteService.SearchNotesAsync(userId, q));
    }

    [HttpGet("category/{category}")]
    public async Task<ActionResult<IEnumerable<NoteDto>>> GetNotesByCategory(string category)
    {
        var userId = await _currentUserService.GetCurrentUserIdAsync();
        return Ok(await _noteService.GetNotesByCategoryAsync(userId, category));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<NoteDto>> GetNote(int id)
    {
        var userId = await _currentUserService.GetCurrentUserIdAsync();
        var note = await _noteService.GetNoteByIdAsync(id, userId);
        if (note == null) return NotFound();
        return Ok(note);
    }

    [HttpPost]
    public async Task<ActionResult<NoteDto>> CreateNote([FromBody] CreateNoteDto createNoteDto)
    {
        var userId = await _currentUserService.GetCurrentUserIdAsync();
        var created = await _noteService.CreateNoteAsync(userId, createNoteDto);
        return CreatedAtAction(nameof(GetNote), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<NoteDto>> UpdateNote(int id, [FromBody] UpdateNoteDto updateNoteDto)
    {
        var userId = await _currentUserService.GetCurrentUserIdAsync();
        var updated = await _noteService.UpdateNoteAsync(id, userId, updateNoteDto);
        return Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteNote(int id)
    {
        var userId = await _currentUserService.GetCurrentUserIdAsync();
        var result = await _noteService.DeleteNoteAsync(id, userId);
        if (!result) return NotFound();
        return NoContent();
    }

    [HttpPatch("{id}/pin")]
    public async Task<ActionResult<NoteDto>> TogglePin(int id)
    {
        var userId = await _currentUserService.GetCurrentUserIdAsync();
        return Ok(await _noteService.TogglePinNoteAsync(id, userId));
    }

    [HttpPatch("{id}/archive")]
    public async Task<ActionResult<NoteDto>> ToggleArchive(int id)
    {
        var userId = await _currentUserService.GetCurrentUserIdAsync();
        return Ok(await _noteService.ToggleArchiveNoteAsync(id, userId));
    }
}
