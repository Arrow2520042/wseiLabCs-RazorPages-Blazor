using ApplicationCore.Enums;

namespace ApplicationCore.Dto;

public abstract record ContactBaseDto
{
    public Guid Id { get; init; }
    public string Email { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
    public AddressDto? Address { get; init; }
    public ContactStatus Status { get; init; }
    public List<string> Tags { get; init; } = new();
    public List<NoteDto> Notes { get; init; } = new();
    public DateTime CreatedAt { get; init; }
}

public record ContactSearchDto(
    string? Query,
    ContactStatus? Status,
    string? Tag,
    string? ContactType,
    int Page = 1,
    int PageSize = 20
);
