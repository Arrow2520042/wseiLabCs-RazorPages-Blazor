using ApplicationCore.Enums;
using ApplicationCore.Models.ContactAggregate;
using Infrastructure.EntityFramework.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Seed;

public class ContactsDbSeeder : IDataSeeder
{
    public int Order => 2;

    private readonly ContactsDbContext _context;
    private readonly ILogger<ContactsDbSeeder> _logger;

    public ContactsDbSeeder(ContactsDbContext context, ILogger<ContactsDbSeeder> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        await SeedPeopleAsync();
    }

    private async Task SeedPeopleAsync()
    {
        var people = new[]
        {
            new Person
            {
                Id = Guid.Parse("7D7A4FFB-9CB4-4D3D-8D79-4A1534A3F100"),
                FirstName = "Jan",
                LastName = "Kowalski",
                Gender = Gender.Male,
                Status = ContactStatus.Active,
                Email = "jan.kowalski@crm.pl",
                Phone = "500200300",
                BirthDate = new DateTime(1990, 6, 12),
                Position = "Sales Manager",
                CreatedAt = DateTime.UtcNow
            },
            new Person
            {
                Id = Guid.Parse("7D7A4FFB-9CB4-4D3D-8D79-4A1534A3F101"),
                FirstName = "Anna",
                LastName = "Nowak",
                Gender = Gender.Female,
                Status = ContactStatus.Active,
                Email = "anna.nowak@crm.pl",
                Phone = "500200301",
                BirthDate = new DateTime(1994, 3, 4),
                Position = "Sales Specialist",
                CreatedAt = DateTime.UtcNow
            },
            new Person
            {
                Id = Guid.Parse("7D7A4FFB-9CB4-4D3D-8D79-4A1534A3F102"),
                FirstName = "Maria",
                LastName = "Wojcik",
                Gender = Gender.Female,
                Status = ContactStatus.Active,
                Email = "maria.wojcik@crm.pl",
                Phone = "500200302",
                BirthDate = new DateTime(1988, 11, 21),
                Position = "Support Agent",
                CreatedAt = DateTime.UtcNow
            }
        };

        foreach (var person in people)
        {
            if (await _context.People.AnyAsync(p => p.Email == person.Email))
                continue;

            await _context.People.AddAsync(person);
            _logger.LogInformation("Dodano seederem kontakt: {Email}", person.Email);
        }

        await _context.SaveChangesAsync();
    }
}
