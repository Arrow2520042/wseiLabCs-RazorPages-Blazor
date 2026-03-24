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
        await unitOfWork.Persons.RemoveByIdAsync(id);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task AddNoteAsync(Guid id, string content)
    {
        var person = await unitOfWork.Persons.FindByIdAsync(id)
            ?? throw new KeyNotFoundException($"Person with id {id} not found.");
        person.Notes.Add(new Note { Content = content });
        await unitOfWork.Persons.UpdateAsync(person);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task AddTagAsync(Guid id, string tag)
    {
        var person = await unitOfWork.Persons.FindByIdAsync(id)
            ?? throw new KeyNotFoundException($"Person with id {id} not found.");
        if (!person.Tags.Contains(tag))
            person.Tags.Add(tag);
        await unitOfWork.Persons.UpdateAsync(person);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task RemoveTagAsync(Guid id, string tag)
    {
        var person = await unitOfWork.Persons.FindByIdAsync(id)
            ?? throw new KeyNotFoundException($"Person with id {id} not found.");
        person.Tags.Remove(tag);
        await unitOfWork.Persons.UpdateAsync(person);
        await unitOfWork.SaveChangesAsync();
    }

    private static async IAsyncEnumerable<T> ToAsyncEnumerable<T>(IEnumerable<T> source)
    {
        foreach (var item in source)
            yield return item;
        await Task.CompletedTask;
    }
}
