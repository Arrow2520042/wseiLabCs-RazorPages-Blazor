using ApplicationCore.Enums;
using ApplicationCore.Interfaces.Repositories;
using ApplicationCore.Models.ContactAggregate;

namespace Infrastructure.Memory.Repositories;

public class MemoryPersonRepository : MemoryGenericRepositoryAsync<Person>, IPersonRepository
{
    public MemoryPersonRepository() : base()
    {
        var person1Id = Guid.Parse("2fa4ea0f-4f53-4e49-89da-bc4db57dc57f");
        _data.Add(person1Id, new Person
        {
            Id = person1Id,
            FirstName = "Kacper",
            LastName = "Wroblewski",
            Email = "kacper.wroblewski@example.com",
            Phone = "+48 501 243 778",
            Position = "Account Manager",
            BirthDate = new DateTime(1993, 5, 14),
            Gender = Gender.Male,
            Status = ApplicationCore.Enums.ContactStatus.Active,
            Tags = ["priority", "b2b"],
            Notes =
            [
                new Note { Content = "Preferuje kontakt po godzinie 10:00." }
            ],
            Address = new Address
            {
                Street = "ul. Lipowa 12/4",
                City = "Krakow",
                PostalCode = "30-001",
                Country = "Polska",
                Type = AddressType.Home
            }
        });

        var person2Id = Guid.Parse("25f35276-a294-4d9e-b431-812afca7ba65");
        _data.Add(person2Id, new Person
        {
            Id = person2Id,
            FirstName = "Alicja",
            LastName = "Jankowska",
            Email = "alicja.jankowska@example.com",
            Phone = "+48 602 115 900",
            Position = "HR Specialist",
            BirthDate = new DateTime(1989, 11, 3),
            Gender = Gender.Female,
            Status = ApplicationCore.Enums.ContactStatus.Active,
            Tags = ["hr"],
            Address = new Address
            {
                Street = "ul. Brzozowa 7",
                City = "Wroclaw",
                PostalCode = "50-210",
                Country = "Polska",
                Type = AddressType.Work
            }
        });

        var person3Id = Guid.Parse("aaf31a7c-d6c5-4c38-a845-dcbfdaf95af4");
        _data.Add(person3Id, new Person
        {
            Id = person3Id,
            FirstName = "Marek",
            LastName = "Czajka",
            Email = "marek.czajka@example.com",
            Phone = "+48 697 880 441",
            Position = "Backend Developer",
            BirthDate = new DateTime(1996, 2, 28),
            Gender = Gender.Male,
            Status = ApplicationCore.Enums.ContactStatus.Inactive,
            Tags = ["tech", "legacy"],
            Notes =
            [
                new Note { Content = "Wznowic kontakt w kolejnym kwartale." }
            ],
            Address = new Address
            {
                Street = "ul. Klonowa 2",
                City = "Poznan",
                PostalCode = "60-112",
                Country = "Polska",
                Type = AddressType.Home
            }
        });

        var person4Id = Guid.Parse("3d4f9239-3586-433a-bee0-a0858634f8f8");
        _data.Add(person4Id, new Person
        {
            Id = person4Id,
            FirstName = "Natalia",
            LastName = "Pawlik",
            Email = "natalia.pawlik@example.com",
            Phone = "+48 730 410 521",
            Position = "Sales Director",
            BirthDate = new DateTime(1985, 8, 21),
            Gender = Gender.Female,
            Status = ApplicationCore.Enums.ContactStatus.Active,
            Tags = ["vip", "sales"],
            Address = new Address
            {
                Street = "ul. Miodowa 18",
                City = "Gdansk",
                PostalCode = "80-180",
                Country = "Polska",
                Type = AddressType.Work
            }
        });
    }

    public Task<IEnumerable<Person>> FindByEmployerAsync(Guid companyId)
    {
        var result = _data.Values.Where(p => p.EmployerId == companyId);
        return Task.FromResult(result);
    }

    public Task<IEnumerable<Person>> FindByOrganizationAsync(Guid organizationId)
    {
        var result = _data.Values.Where(p => p.OrganizationId == organizationId);
        return Task.FromResult(result);
    }
}
