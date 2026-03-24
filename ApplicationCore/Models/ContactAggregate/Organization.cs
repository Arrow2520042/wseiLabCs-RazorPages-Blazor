using ApplicationCore.Enums;

namespace ApplicationCore.Models.ContactAggregate;

public class Organization : Contact
{
    public string Name { get; set; } = string.Empty;
    public OrganizationType Type { get; set; }
    public string? Description { get; set; }
}
