using ApplicationCore.Dto;
using ApplicationCore.Models.ContactAggregate;

namespace ApplicationCore.Interfaces.Services;

public interface IPersonService
{
    Task<PagedResult<PersonDto>> FindAllPeoplePaged(int page, int size);
    Task<IAsyncEnumerable<PersonDto>> FindPeopleFromCompany(Guid companyId);
    Task<Person> AddPerson(CreatePersonDto personDto);
    Task<Person> UpdatePerson(Guid id, UpdatePersonDto personDto);
    Task<PersonDto?> GetById(Guid id);
    Task DeletePersonAsync(Guid id);
    Task AddNoteAsync(Guid id, string content);
    Task AddTagAsync(Guid id, string tag);
    Task RemoveTagAsync(Guid id, string tag);
}
