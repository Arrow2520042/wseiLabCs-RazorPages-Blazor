using ApplicationCore.Enums;

namespace ApplicationCore.Models.ContactAggregate;

public class Person : Contact
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Position { get; set; }
    public DateTime? BirthDate { get; set; }
    public Gender Gender { get; set; }
    public Guid? EmployerId { get; set; }
    public Company? Employer { get; set; }
    public Guid? OrganizationId { get; set; }
    public Organization? Organization { get; set; }
}
