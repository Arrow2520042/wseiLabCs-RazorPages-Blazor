using ApplicationCore.Dto;
using ApplicationCore.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace BackendLab01.Controllers;

[ApiController]
[Route("/api/contacts")]
public class ContactsController(IPersonService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllPersons([FromQuery] int page = 1, [FromQuery] int size = 20)
    {
        return Ok(await service.FindAllPeoplePaged(page, size));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetPerson(Guid id)
    {
        var dto = await service.GetById(id);
        return dto is not null ? Ok(dto) : NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePersonDto dto)
    {
        var result = await service.AddPerson(dto);
        return CreatedAtAction(nameof(GetPerson), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePersonDto dto)
    {
        var existing = await service.GetById(id);
        if (existing is null)
            return NotFound();

        var result = await service.UpdatePerson(id, dto);
        var personDto = await service.GetById(id);
        return Ok(personDto);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await service.DeletePersonAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost("{id:guid}/tags")]
    public async Task<IActionResult> AddTag(Guid id, [FromBody] string tag)
    {
        try
        {
            await service.AddTagAsync(id, tag);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id:guid}/tags/{tag}")]
    public async Task<IActionResult> RemoveTag(Guid id, string tag)
    {
        try
        {
            await service.RemoveTagAsync(id, tag);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost("{id:guid}/notes")]
    public async Task<IActionResult> AddNote(Guid id, [FromBody] string content)
    {
        try
        {
            await service.AddNoteAsync(id, content);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
