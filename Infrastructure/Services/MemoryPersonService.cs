using ApplicationCore.Dto;
using ApplicationCore.Interfaces;
using ApplicationCore.Interfaces.Services;
using ApplicationCore.Models.ContactAggregate;
using AutoMapper;

namespace Infrastructure.Services;

public class MemoryPersonService(IContactUnitOfWork unitOfWork, IMapper mapper) : IPersonService
{
    public async Task<PagedResult<PersonDto>> FindAllPeoplePaged(int page, int size)
    {
        var paged = await unitOfWork.Persons.FindPagedAsync(page, size);
        var items = paged.Items.Select(mapper.Map<PersonDto>).ToList();
        return new PagedResult<PersonDto>(items, paged.TotalCount, paged.Page, paged.PageSize);
    }

    public async Task<IAsyncEnumerable<PersonDto>> FindPeopleFromCompany(Guid companyId)
    {
        var people = await unitOfWork.Persons.FindByEmployerAsync(companyId);
        return ToAsyncEnumerable(people.Select(mapper.Map<PersonDto>));
    }

    public async Task<Person> AddPerson(CreatePersonDto personDto)
    {
        var entity = mapper.Map<Person>(personDto);
        entity = await unitOfWork.Persons.AddAsync(entity);
        await unitOfWork.SaveChangesAsync();
        return entity;
    }

    public async Task<Person> UpdatePerson(Guid id, UpdatePersonDto personDto)
    {
        var entity = await unitOfWork.Persons.FindByIdAsync(id)
            ?? throw new KeyNotFoundException($"Person with id {id} not found.");
        mapper.Map(personDto, entity);
        entity = await unitOfWork.Persons.UpdateAsync(entity);
        await unitOfWork.SaveChangesAsync();
        return entity;
    }

    public async Task<PersonDto?> GetById(Guid id)
    {
        var person = await unitOfWork.Persons.FindByIdAsync(id);
        return person is not null ? mapper.Map<PersonDto>(person) : null;
    }

    public async Task DeletePersonAsync(Guid id)
    {
        try
        {
            await unitOfWork.Persons.RemoveByIdAsync(id);
            await unitOfWork.SaveChangesAsync();
        }
        catch (KeyNotFoundException)
        {
            throw new ApplicationCore.Exceptions.ContactNotFoundException($"Person with id {id} was not found.");
        }
    }

    public async Task AddNoteAsync(Guid id, string content)
    {
        var person = await unitOfWork.Persons.FindByIdAsync(id)
            ?? throw new ApplicationCore.Exceptions.ContactNotFoundException($"Person with id {id} not found.");
        person.Notes.Add(new Note { Id = Guid.NewGuid(), Content = content, CreatedAt = DateTime.UtcNow });
        await unitOfWork.Persons.UpdateAsync(person);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task AddTagAsync(Guid id, string tag)
    {
        var person = await unitOfWork.Persons.FindByIdAsync(id)
            ?? throw new ApplicationCore.Exceptions.ContactNotFoundException($"Person with id {id} not found.");
        if (!person.Tags.Contains(tag))
            person.Tags.Add(tag);
        await unitOfWork.Persons.UpdateAsync(person);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task RemoveTagAsync(Guid id, string tag)
    {
        var person = await unitOfWork.Persons.FindByIdAsync(id)
            ?? throw new ApplicationCore.Exceptions.ContactNotFoundException($"Person with id {id} not found.");
        person.Tags.Remove(tag);
        await unitOfWork.Persons.UpdateAsync(person);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task<NoteDto> AddNoteToPerson(Guid personId, CreateNoteDto noteDto)
    {
        var person = await unitOfWork.Persons.FindByIdAsync(personId)
                     ?? throw new ApplicationCore.Exceptions.ContactNotFoundException($"Person with id {personId} was not found.");

        person.Notes ??= new List<Note>();

        var note = mapper.Map<Note>(noteDto);
        person.Notes.Add(note);

        await unitOfWork.Persons.UpdateAsync(person);
        await unitOfWork.SaveChangesAsync();

        return mapper.Map<NoteDto>(note);
    }

    public async Task<IEnumerable<NoteDto>> GetNotes(Guid personId)
    {
        var person = await unitOfWork.Persons.FindByIdAsync(personId)
                     ?? throw new ApplicationCore.Exceptions.ContactNotFoundException($"Person with id {personId} was not found.");

        return person.Notes.Select(mapper.Map<NoteDto>).ToList();
    }

    public async Task RemoveNoteFromPerson(Guid personId, Guid noteId)
    {
        var person = await unitOfWork.Persons.FindByIdAsync(personId)
                     ?? throw new ApplicationCore.Exceptions.ContactNotFoundException($"Person with id {personId} was not found.");

        var note = person.Notes.FirstOrDefault(n => n.Id == noteId);
        if (note is null)
            throw new ApplicationCore.Exceptions.NoteNotFoundException($"Note with id {noteId} was not found for person {personId}.");

        person.Notes.Remove(note);
        await unitOfWork.Persons.UpdateAsync(person);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task<PagedResult<PersonDto>> SearchPeople(ContactSearchDto search)
    {
        var all = (await unitOfWork.Persons.FindAllAsync()).AsQueryable();

        if (!string.IsNullOrWhiteSpace(search.Query))
        {
            var query = search.Query.Trim().ToLowerInvariant();
            all = all.Where(p =>
                p.FirstName.ToLowerInvariant().Contains(query) ||
                p.LastName.ToLowerInvariant().Contains(query) ||
                p.Email.ToLowerInvariant().Contains(query) ||
                (p.Position ?? string.Empty).ToLowerInvariant().Contains(query));
        }

        if (search.Status.HasValue)
            all = all.Where(p => p.Status == search.Status.Value);

        if (!string.IsNullOrWhiteSpace(search.Tag))
            all = all.Where(p => p.Tags.Any(t => t.Equals(search.Tag.Trim(), StringComparison.OrdinalIgnoreCase)));

        var total = all.Count();
        var page = Math.Max(search.Page, 1);
        var pageSize = Math.Clamp(search.PageSize, 1, 200);

        var items = all
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(mapper.Map<PersonDto>)
            .ToList();

        return new PagedResult<PersonDto>(items, total, page, pageSize);
    }

    private static async IAsyncEnumerable<T> ToAsyncEnumerable<T>(IEnumerable<T> source)
    {
        foreach (var item in source)
            yield return item;
        await Task.CompletedTask;
    }
}
