using ApplicationCore.Enums;

namespace ApplicationCore.Dto;

public record CompanyDto : ContactBaseDto
{
    public string Name { get; init; } = string.Empty;
    public string Nip { get; init; } = string.Empty;
    public string? Website { get; init; }
}

public record CreateCompanyDto(
    string Name,
    string Nip,
    string Email,
    string Phone,
    string? Website,
    AddressDto? Address
);

public record UpdateCompanyDto(
    string? Name,
    string? Nip,
    string? Email,
    string? Phone,
    string? Website,
    AddressDto? Address,
    ContactStatus? Status
);
