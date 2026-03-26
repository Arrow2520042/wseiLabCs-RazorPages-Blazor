using ApplicationCore.Dto;
using ApplicationCore.Enums;
using ApplicationCore.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace BackendLab01.Controllers;

[ApiController]
[Route("api/contacts")]
public class ContactsController : ControllerBase
{
    private readonly IPersonService _service;
    private readonly ILogger<ContactsController> _logger;

    public ContactsController(IPersonService service, ILogger<ContactsController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllPersons(
        [FromQuery] int page = 1,
        [FromQuery] int size = 20,
        [FromQuery] string? query = null,
        [FromQuery] ContactStatus? status = null,
        [FromQuery] string? tag = null)
    {
        if (page <= 0 || size <= 0)
            return BadRequest("Page and size must be greater than 0.");

        var result = await _service.SearchPeople(new ContactSearchDto(query, status, tag, null, page, size));
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPerson(Guid id)
    {
        var dto = await _service.GetById(id);
        return dto is not null ? Ok(dto) : NotFound();
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreatePersonDto dto)
    {
        var entity = await _service.AddPerson(dto);
        var createdDto = await _service.GetById(entity.Id);
        return CreatedAtAction(nameof(GetPerson), new { id = entity.Id }, createdDto);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePersonDto dto)
    {
        var existing = await _service.GetById(id);
        if (existing is null)
            return NotFound();

        await _service.UpdatePerson(id, dto);
        var updatedDto = await _service.GetById(id);
        return Ok(updatedDto);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.DeletePersonAsync(id);
        return NoContent();
    }

    [HttpPost("{id:guid}/tags")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddTag(Guid id, [FromBody] string tag)
    {
        await _service.AddTagAsync(id, tag);
        return NoContent();
    }

    [HttpDelete("{id:guid}/tags/{tag}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveTag(Guid id, string tag)
    {
        await _service.RemoveTagAsync(id, tag);
        return NoContent();
    }

    [HttpPost("{id:guid}/notes")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddNote(Guid id, [FromBody] CreateNoteDto dto)
    {
        var note = await _service.AddNoteToPerson(id, dto);
        return CreatedAtAction(nameof(GetNotes), new { id }, note);
    }

    [HttpGet("{id:guid}/notes")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetNotes(Guid id)
    {
        var notes = await _service.GetNotes(id);
        return Ok(notes);
    }

    [HttpDelete("{id:guid}/notes/{noteId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteNote(Guid id, Guid noteId)
    {
        await _service.RemoveNoteFromPerson(id, noteId);
        return NoContent();
    }
}
