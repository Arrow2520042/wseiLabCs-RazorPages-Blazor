using ApplicationCore.Enums;

namespace ApplicationCore.Dto;

public record OrganizationDto : ContactBaseDto
{
    public string Name { get; init; } = string.Empty;
    public OrganizationType Type { get; init; }
    public string? Description { get; init; }
}

public record CreateOrganizationDto(
    string Name,
    OrganizationType Type,
    string Email,
    string Phone,
    string? Description,
    AddressDto? Address
);

public record UpdateOrganizationDto(
    string? Name,
    OrganizationType? Type,
    string? Email,
    string? Phone,
    string? Description,
    AddressDto? Address,
    ContactStatus? Status
);
