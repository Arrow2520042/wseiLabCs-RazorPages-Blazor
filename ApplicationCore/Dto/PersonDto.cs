using ApplicationCore.Enums;
using ApplicationCore.Models.ContactAggregate;

namespace ApplicationCore.Dto;

public record PersonDto : ContactBaseDto
{
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public string? Position { get; init; }
    public DateTime? BirthDate { get; init; }
    public Gender Gender { get; init; }
    public Guid? EmployerId { get; init; }
    public string? EmployerName { get; init; }

    public static PersonDto FromEntity(Person person) => new()
    {
        Id = person.Id,
        FirstName = person.FirstName,
        LastName = person.LastName,
        FullName = $"{person.FirstName} {person.LastName}",
        Email = person.Email,
        Phone = person.Phone,
        Position = person.Position,
        BirthDate = person.BirthDate,
        Gender = person.Gender,
        EmployerId = person.EmployerId,
        EmployerName = person.Employer?.Name,
        Status = person.Status,
        Tags = person.Tags,
        CreatedAt = person.CreatedAt,
        Address = person.Address is not null ? AddressDto.FromEntity(person.Address) : null
    };

    public static implicit operator PersonDto(Person person) => FromEntity(person);
}

public record CreatePersonDto(
    string FirstName,
    string LastName,
    string Email,
    string Phone,
    string? Position,
    DateTime? BirthDate,
    Gender Gender,
    Guid? EmployerId,
    AddressDto? Address
)
{
    public Person ToEntity() => new()
    {
        Id = Guid.NewGuid(),
        FirstName = FirstName,
        LastName = LastName,
        Email = Email,
        Phone = Phone,
        Position = Position,
        BirthDate = BirthDate,
        Gender = Gender,
        EmployerId = EmployerId,
        Address = Address?.ToEntity(),
        CreatedAt = DateTime.UtcNow
    };

    public static explicit operator Person(CreatePersonDto dto) => dto.ToEntity();
}

public record UpdatePersonDto(
    string? FirstName,
    string? LastName,
    string? Email,
    string? Phone,
    string? Position,
    DateTime? BirthDate,
    Gender? Gender,
    Guid? EmployerId,
    AddressDto? Address,
    ContactStatus? Status
)
{
    public void ApplyTo(Person person)
    {
        if (FirstName is not null) person.FirstName = FirstName;
        if (LastName is not null) person.LastName = LastName;
        if (Email is not null) person.Email = Email;
        if (Phone is not null) person.Phone = Phone;
        if (Position is not null) person.Position = Position;
        if (BirthDate is not null) person.BirthDate = BirthDate;
        if (Gender is not null) person.Gender = Gender.Value;
        if (EmployerId is not null) person.EmployerId = EmployerId;
        if (Address is not null) person.Address = Address.ToEntity();
        if (Status is not null) person.Status = Status.Value;
    }
};
