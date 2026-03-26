using ApplicationCore.Dto;
using ApplicationCore.Interfaces;
using ApplicationCore.Interfaces.Repositories;
using ApplicationCore.Interfaces.Services;
using ApplicationCore.Mapper;
using ApplicationCore.Models.ContactAggregate;
using AutoMapper;
using Infrastructure.Memory;
using Infrastructure.Memory.Repositories;
using Infrastructure.Services;

namespace UnitTest;

public class MemoryPersonServiceTest
{
    private readonly IPersonService _service;

    public MemoryPersonServiceTest()
    {
        var personRepo = new MemoryPersonRepository();
        var companyRepo = new MemoryCompanyRepository();
        var organizationRepo = new MemoryOrganizationRepository();
        var unitOfWork = new MemoryContactUnitOfWork(personRepo, companyRepo, organizationRepo);

        var configExpression = new AutoMapper.MapperConfigurationExpression();
        configExpression.AddProfile<ContactsMappingProfile>();

        var mapperConfig = new AutoMapper.MapperConfiguration(configExpression, Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory.Instance);
        var mapper = new AutoMapper.Mapper(mapperConfig);

        _service = new MemoryPersonService(unitOfWork, mapper);
    }

    [Fact]
    public async Task AddNoteToPerson_ShouldReturnNoteDto()
    {
        var person = await _service.AddPerson(new CreatePersonDto("Ewa", "Kowalska", "ewa@example.com", "+48 123 456 789", null, null, ApplicationCore.Enums.Gender.Female, null, null));

        var note = await _service.AddNoteToPerson(person.Id, new CreateNoteDto("Test note"));

        Assert.NotNull(note);
        Assert.Equal("Test note", note.Content);
        var notes = await _service.GetNotes(person.Id);
        Assert.Contains(notes, n => n.Id == note.Id && n.Content == "Test note");
    }

    [Fact]
    public async Task SearchPeople_ShouldSupportQueryAndPaging()
    {
        await _service.AddPerson(new CreatePersonDto("Jan", "Nowak", "jan.nowak@example.com", "+48 111 111 111", null, null, ApplicationCore.Enums.Gender.Male, null, null));
        await _service.AddPerson(new CreatePersonDto("Anna", "Nowak", "anna.nowak@example.com", "+48 222 222 222", null, null, ApplicationCore.Enums.Gender.Female, null, null));

        var result = await _service.SearchPeople(new ContactSearchDto("Anna", null, null, null, 1, 10));

        Assert.Single(result.Items);
        Assert.Equal("Anna", result.Items.First().FirstName);
    }
}
