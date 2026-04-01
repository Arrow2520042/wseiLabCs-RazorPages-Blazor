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
        var person = await _service.AddPerson(new CreatePersonDto("Alice", "Johnson", "alice.johnson@example.com", "+48 987 654 321", null, null, ApplicationCore.Enums.Gender.Female, null, null));

        var note = await _service.AddNoteToPerson(person.Id, new CreateNoteDto("Sample note"));

        Assert.NotNull(note);
        Assert.Equal("Sample note", note.Content);
        var notes = await _service.GetNotes(person.Id);
        Assert.Contains(notes, n => n.Id == note.Id && n.Content == "Sample note");
    }

    [Fact]
    public async Task SearchPeople_ShouldSupportQueryAndPaging()
    {
        await _service.AddPerson(new CreatePersonDto("Bob", "Brown", "bob.brown@example.com", "+48 123 456 789", null, null, ApplicationCore.Enums.Gender.Male, null, null));
        await _service.AddPerson(new CreatePersonDto("Charlie", "Davis", "charlie.davis@example.com", "+48 321 654 987", null, null, ApplicationCore.Enums.Gender.Male, null, null));

        var result = await _service.SearchPeople(new ContactSearchDto("Charlie", null, null, null, 1, 10));

        Assert.Single(result.Items);
        Assert.Equal("Charlie", result.Items.First().FirstName);
    }
}
