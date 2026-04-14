using System.Text.Json;
using ApplicationCore.Enums;
using ApplicationCore.Models.ContactAggregate;
using Infrastructure.EntityFramework.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.EntityFramework.Context;

public class ContactsDbContext : IdentityDbContext<CrmUser, CrmRole, string>
{
    public DbSet<Contact> Contacts { get; set; } = null!;
    public DbSet<Person> People { get; set; } = null!;
    public DbSet<Company> Companies { get; set; } = null!;
    public DbSet<Organization> Organizations { get; set; } = null!;

    public ContactsDbContext()
    {
    }

    public ContactsDbContext(DbContextOptions<ContactsDbContext> options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite("Data Source=contacts.db");
        }
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        ConfigureIdentity(builder);
        ConfigureContacts(builder);
        SeedIdentity(builder);
        SeedContacts(builder);
    }

    private static void ConfigureIdentity(ModelBuilder builder)
    {
        builder.Entity<CrmUser>(entity =>
        {
            entity.Property(u => u.FirstName).HasMaxLength(100).IsRequired();
            entity.Property(u => u.LastName).HasMaxLength(100).IsRequired();
            entity.Property(u => u.FullName).HasMaxLength(200).IsRequired();
            entity.Property(u => u.Department).HasMaxLength(100).IsRequired();
            entity.Property(u => u.Status).HasConversion<string>().HasMaxLength(30);
            entity.Property(u => u.Email).HasMaxLength(200);
            entity.HasIndex(u => u.Email).IsUnique();
        });

        builder.Entity<CrmRole>(entity =>
        {
            entity.Property(r => r.Name).HasMaxLength(30);
        });
    }

    private static void ConfigureContacts(ModelBuilder builder)
    {
        var tagsConverter = new ValueConverter<List<string>, string>(
            value => JsonSerializer.Serialize(value, (JsonSerializerOptions?)null),
            value => string.IsNullOrWhiteSpace(value)
                ? new List<string>()
                : JsonSerializer.Deserialize<List<string>>(value, (JsonSerializerOptions?)null) ?? new List<string>());

        var tagsComparer = new ValueComparer<List<string>>(
            (left, right) => (left ?? new List<string>()).SequenceEqual(right ?? new List<string>()),
            value => value == null
                ? 0
                : value.Aggregate(0, (hash, item) => HashCode.Combine(hash, item.GetHashCode())),
            value => value == null ? new List<string>() : value.ToList());

        var notesConverter = new ValueConverter<List<Note>, string>(
            value => JsonSerializer.Serialize(value, (JsonSerializerOptions?)null),
            value => string.IsNullOrWhiteSpace(value)
                ? new List<Note>()
                : JsonSerializer.Deserialize<List<Note>>(value, (JsonSerializerOptions?)null) ?? new List<Note>());

        var notesComparer = new ValueComparer<List<Note>>(
            (left, right) => SerializeNotes(left) == SerializeNotes(right),
            value => SerializeNotes(value).GetHashCode(),
            value => value == null
                ? new List<Note>()
                : value.Select(n => new Note { Id = n.Id, Content = n.Content, CreatedAt = n.CreatedAt }).ToList());

        builder.Entity<Contact>()
            .HasDiscriminator<string>("ContactType")
            .HasValue<Person>("Person")
            .HasValue<Company>("Company")
            .HasValue<Organization>("Organization");

        builder.Entity<Contact>(entity =>
        {
            entity.Property(c => c.Email).HasMaxLength(200).IsRequired();
            entity.Property(c => c.Phone).HasMaxLength(20).IsRequired();
            entity.Property(c => c.Status).HasConversion<string>().HasMaxLength(30);

            entity.Property(c => c.Tags)
                .HasConversion(tagsConverter)
                .HasColumnType("TEXT")
                .Metadata.SetValueComparer(tagsComparer);

            entity.Property(c => c.Notes)
                .HasConversion(notesConverter)
                .HasColumnType("TEXT")
                .Metadata.SetValueComparer(notesComparer);
        });

        builder.Entity<Contact>().OwnsOne(
            c => c.Address,
            owned =>
            {
                owned.Property(a => a.Street).HasMaxLength(200);
                owned.Property(a => a.City).HasMaxLength(100);
                owned.Property(a => a.PostalCode).HasMaxLength(12);
                owned.Property(a => a.Country).HasMaxLength(100);
                owned.Property(a => a.Type).HasConversion<string>().HasMaxLength(30);
            });

        builder.Entity<Person>(entity =>
        {
            entity.Property(p => p.FirstName).HasMaxLength(100).IsRequired();
            entity.Property(p => p.LastName).HasMaxLength(200).IsRequired();
            entity.Property(p => p.Position).HasMaxLength(120);
            entity.Property(p => p.BirthDate).HasColumnType("date");
            entity.Property(p => p.Gender).HasConversion<string>().HasMaxLength(20);
        });

        builder.Entity<Company>(entity =>
        {
            entity.Property(c => c.Name).HasMaxLength(200).IsRequired();
            entity.Property(c => c.Nip).HasMaxLength(20).IsRequired();
            entity.Property(c => c.Website).HasMaxLength(250);
            entity.HasIndex(c => c.Nip).IsUnique();
        });

        builder.Entity<Organization>(entity =>
        {
            entity.Property(o => o.Name).HasMaxLength(200).IsRequired();
            entity.Property(o => o.Type).HasConversion<string>().HasMaxLength(40);
            entity.Property(o => o.Description).HasMaxLength(500);
        });

        builder.Entity<Person>()
            .HasOne(p => p.Employer)
            .WithMany(c => c.Employees)
            .HasForeignKey(p => p.EmployerId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<Organization>()
            .HasMany(o => o.Members)
            .WithOne(p => p.Organization)
            .HasForeignKey(p => p.OrganizationId)
            .OnDelete(DeleteBehavior.SetNull);
    }

    private static void SeedIdentity(ModelBuilder builder)
    {
        const string adminRoleId = "6B4EE8AB-8BA8-4DDF-8E8E-CC0AF1D3AB10";
        const string salesManagerRoleId = "6B4EE8AB-8BA8-4DDF-8E8E-CC0AF1D3AB11";
        const string salesRoleId = "6B4EE8AB-8BA8-4DDF-8E8E-CC0AF1D3AB12";
        const string supportRoleId = "6B4EE8AB-8BA8-4DDF-8E8E-CC0AF1D3AB13";
        const string readOnlyRoleId = "6B4EE8AB-8BA8-4DDF-8E8E-CC0AF1D3AB14";

        builder.Entity<CrmRole>().HasData(
            new CrmRole { Id = adminRoleId, Name = UserRole.Administrator.ToString(), NormalizedName = UserRole.Administrator.ToString().ToUpperInvariant(), Description = "System administrator" },
            new CrmRole { Id = salesManagerRoleId, Name = UserRole.SalesManager.ToString(), NormalizedName = UserRole.SalesManager.ToString().ToUpperInvariant(), Description = "Sales manager" },
            new CrmRole { Id = salesRoleId, Name = UserRole.Salesperson.ToString(), NormalizedName = UserRole.Salesperson.ToString().ToUpperInvariant(), Description = "Sales representative" },
            new CrmRole { Id = supportRoleId, Name = UserRole.SupportAgent.ToString(), NormalizedName = UserRole.SupportAgent.ToString().ToUpperInvariant(), Description = "Support employee" },
            new CrmRole { Id = readOnlyRoleId, Name = UserRole.ReadOnly.ToString(), NormalizedName = UserRole.ReadOnly.ToString().ToUpperInvariant(), Description = "Read-only access" }
        );

        const string adminUserId = "6D89BC32-15F1-4E30-AF25-53F1B4429A10";
        const string salesUserId = "6D89BC32-15F1-4E30-AF25-53F1B4429A11";

        var createdAt = new DateTime(2026, 3, 31, 8, 0, 0, DateTimeKind.Utc);

        builder.Entity<CrmUser>().HasData(
            new CrmUser
            {
                Id = adminUserId,
                UserName = "admin@wsei.edu.pl",
                NormalizedUserName = "ADMIN@WSEI.EDU.PL",
                Email = "admin@wsei.edu.pl",
                NormalizedEmail = "ADMIN@WSEI.EDU.PL",
                EmailConfirmed = true,
                FirstName = "System",
                LastName = "Admin",
                FullName = "System Admin",
                Department = "IT",
                Status = SystemUserStatus.Active,
                CreatedAt = createdAt,
                SecurityStamp = "SEC-CRM-ADMIN-001",
                ConcurrencyStamp = "CON-CRM-ADMIN-001"
            },
            new CrmUser
            {
                Id = salesUserId,
                UserName = "sales@wsei.edu.pl",
                NormalizedUserName = "SALES@WSEI.EDU.PL",
                Email = "sales@wsei.edu.pl",
                NormalizedEmail = "SALES@WSEI.EDU.PL",
                EmailConfirmed = true,
                FirstName = "Sales",
                LastName = "Manager",
                FullName = "Sales Manager",
                Department = "Sales",
                Status = SystemUserStatus.Active,
                CreatedAt = createdAt,
                SecurityStamp = "SEC-CRM-SALES-001",
                ConcurrencyStamp = "CON-CRM-SALES-001"
            }
        );

        builder.Entity<IdentityUserRole<string>>().HasData(
            new IdentityUserRole<string> { UserId = adminUserId, RoleId = adminRoleId },
            new IdentityUserRole<string> { UserId = salesUserId, RoleId = salesManagerRoleId }
        );
    }

    private static void SeedContacts(ModelBuilder builder)
    {
        var createdAt = new DateTime(2026, 3, 31, 10, 0, 0, DateTimeKind.Utc);

        var companyId = Guid.Parse("516A34D7-CCFB-4F20-85F3-62BD0F3AF271");
        var person1Id = Guid.Parse("3D54091D-ABC8-49EC-9590-93AD3ED5458F");
        var person2Id = Guid.Parse("B4DCB17C-F875-43F8-9D66-36597895A466");

        builder.Entity<Company>().HasData(
            new Company
            {
                Id = companyId,
                Name = "WSEI",
                Nip = "6750000000",
                Phone = "123567123",
                Email = "biuro@wsei.edu.pl",
                Website = "https://wsei.edu.pl",
                Status = ContactStatus.Active,
                CreatedAt = createdAt,
                Tags = new List<string>(),
                Notes = new List<Note>()
            }
        );

        builder.Entity<Person>().HasData(
            new Person
            {
                Id = person1Id,
                FirstName = "Adam",
                LastName = "Nowak",
                Gender = Gender.Male,
                Status = ContactStatus.Active,
                Email = "adam@wsei.edu.pl",
                Phone = "123456789",
                BirthDate = new DateTime(2001, 1, 11),
                Position = "Programista",
                EmployerId = companyId,
                CreatedAt = createdAt,
                Tags = new List<string>(),
                Notes = new List<Note>()
            },
            new Person
            {
                Id = person2Id,
                FirstName = "Ewa",
                LastName = "Kowalska",
                Gender = Gender.Female,
                Status = ContactStatus.Blocked,
                Email = "ewa@wsei.edu.pl",
                Phone = "123123123",
                BirthDate = new DateTime(2001, 1, 11),
                Position = "Tester",
                EmployerId = companyId,
                CreatedAt = createdAt,
                Tags = new List<string>(),
                Notes = new List<Note>()
            }
        );

        builder.Entity<Contact>().OwnsOne(c => c.Address).HasData(
            new
            {
                ContactId = person1Id,
                Street = "ul. Sw. Filipa 17",
                City = "Krakow",
                PostalCode = "25-009",
                Country = "Poland",
                Type = AddressType.Work
            },
            new
            {
                ContactId = person2Id,
                Street = "ul. Dobra 5",
                City = "Krakow",
                PostalCode = "30-001",
                Country = "Poland",
                Type = AddressType.Home
            }
        );
    }

    private static string SerializeNotes(List<Note>? notes)
    {
        return JsonSerializer.Serialize(notes ?? new List<Note>(), (JsonSerializerOptions?)null);
    }
}
