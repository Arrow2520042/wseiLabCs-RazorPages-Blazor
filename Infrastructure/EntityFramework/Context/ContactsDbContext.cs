using System.Text.Json;
using ApplicationCore.Enums;
using ApplicationCore.Models.ContactAggregate;
using Infrastructure.EntityFramework.Entities;
using Infrastructure.Security;
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
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;

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
        ConfigureSecurity(builder);
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

    private static void ConfigureSecurity(ModelBuilder builder)
    {
        builder.Entity<RefreshToken>(entity =>
        {
            entity.ToTable("RefresTokens");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.UserId)
                .IsRequired()
                .HasMaxLength(450);

            entity.Property(x => x.Token)
                .IsRequired()
                .HasMaxLength(512);

            entity.HasIndex(x => x.UserId);
            entity.HasIndex(x => x.Token).IsUnique();

            entity.HasOne<CrmUser>()
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
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
                PasswordHash = "AQAAAAIAAYagAAAAEIUGp/TT1NcVE8M77dDN1R49jG6hYMnAlyOzNFVZsFbxYLXF7djmW9FsFcIh1UyZ5A==",
                LockoutEnabled = true,
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
                PasswordHash = "AQAAAAIAAYagAAAAEMDVL814x0HYV8B6rILlR6sZjGfiS7H/0kQZNVMeYfsLd0pg/nFCQmr7/8zlW6NaEw==",
                LockoutEnabled = true,
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
        var createdAt = new DateTime(2026, 4, 20, 9, 30, 0, DateTimeKind.Utc);

        var companyId = Guid.Parse("8A8ECDE7-0EAA-4E9E-A67F-3AC9857F4E11");
        var organizationId = Guid.Parse("8A8ECDE7-0EAA-4E9E-A67F-3AC9857F4E12");
        var person1Id = Guid.Parse("8A8ECDE7-0EAA-4E9E-A67F-3AC9857F4E21");
        var person2Id = Guid.Parse("8A8ECDE7-0EAA-4E9E-A67F-3AC9857F4E22");
        var person3Id = Guid.Parse("8A8ECDE7-0EAA-4E9E-A67F-3AC9857F4E23");

        builder.Entity<Company>().HasData(
            new Company
            {
                Id = companyId,
                Name = "Baltic Soft Solutions",
                Nip = "5842781123",
                Phone = "583221104",
                Email = "kontakt@balticsoft.pl",
                Website = "https://balticsoft.pl",
                Status = ContactStatus.Active,
                CreatedAt = createdAt,
                Tags = new List<string> { "b2b", "it", "partner" },
                Notes = new List<Note>
                {
                    new() { Id = Guid.Parse("8A8ECDE7-0EAA-4E9E-A67F-3AC9857F5011"), Content = "Klient strategiczny z Pomorza.", CreatedAt = createdAt }
                }
            }
        );

        builder.Entity<Organization>().HasData(
            new Organization
            {
                Id = organizationId,
                Name = "Pomorska Izba Cyfrowa",
                Type = OrganizationType.Other,
                Description = "Siec wspolpracy firm technologicznych.",
                Phone = "583004455",
                Email = "biuro@izbacyfrowa.pl",
                Status = ContactStatus.Active,
                CreatedAt = createdAt,
                Tags = new List<string> { "networking", "wydarzenia" },
                Notes = new List<Note>()
            }
        );

        builder.Entity<Person>().HasData(
            new Person
            {
                Id = person1Id,
                FirstName = "Katarzyna",
                LastName = "Witkowska",
                Gender = Gender.Female,
                Status = ContactStatus.Active,
                Email = "k.witkowska@balticsoft.pl",
                Phone = "501778992",
                BirthDate = new DateTime(1993, 9, 17),
                Position = "Kierownik Projektu",
                EmployerId = companyId,
                OrganizationId = organizationId,
                CreatedAt = createdAt,
                Tags = new List<string> { "decision-maker", "enterprise" },
                Notes = new List<Note>
                {
                    new() { Id = Guid.Parse("8A8ECDE7-0EAA-4E9E-A67F-3AC9857F6011"), Content = "Preferuje kontakt telefoniczny rano.", CreatedAt = createdAt }
                }
            },
            new Person
            {
                Id = person2Id,
                FirstName = "Michal",
                LastName = "Czernecki",
                Gender = Gender.Male,
                Status = ContactStatus.Active,
                Email = "m.czernecki@balticsoft.pl",
                Phone = "603115870",
                BirthDate = new DateTime(1990, 2, 11),
                Position = "Architekt Rozwiazan",
                EmployerId = companyId,
                OrganizationId = organizationId,
                CreatedAt = createdAt,
                Tags = new List<string> { "security", "cloud" },
                Notes = new List<Note>()
            },
            new Person
            {
                Id = person3Id,
                FirstName = "Agnieszka",
                LastName = "Golebiowska",
                Gender = Gender.Female,
                Status = ContactStatus.Inactive,
                Email = "a.golebiowska@izbacyfrowa.pl",
                Phone = "509447221",
                BirthDate = new DateTime(1988, 7, 5),
                Position = "Koordynator Partnerstw",
                OrganizationId = organizationId,
                CreatedAt = createdAt,
                Tags = new List<string> { "events", "ngo" },
                Notes = new List<Note>()
            }
        );

        builder.Entity<Contact>().OwnsOne(c => c.Address).HasData(
            new
            {
                ContactId = person1Id,
                Street = "ul. Grunwaldzka 103A",
                City = "Gdansk",
                PostalCode = "80-244",
                Country = "Polska",
                Type = AddressType.Work
            },
            new
            {
                ContactId = person2Id,
                Street = "al. Jana Pawla II 28",
                City = "Warszawa",
                PostalCode = "00-141",
                Country = "Polska",
                Type = AddressType.Home
            },
            new
            {
                ContactId = person3Id,
                Street = "ul. Pilsudskiego 14",
                City = "Lodz",
                PostalCode = "90-051",
                Country = "Polska",
                Type = AddressType.Work
            }
        );
    }

    private static string SerializeNotes(List<Note>? notes)
    {
        return JsonSerializer.Serialize(notes ?? new List<Note>(), (JsonSerializerOptions?)null);
    }
}
