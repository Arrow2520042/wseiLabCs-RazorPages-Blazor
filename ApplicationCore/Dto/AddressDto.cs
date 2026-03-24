using ApplicationCore.Enums;
using ApplicationCore.Models.ContactAggregate;

namespace ApplicationCore.Dto;

public record AddressDto(
    string Street,
    string City,
    string PostalCode,
    string Country,
    AddressType Type
)
{
    public static AddressDto FromEntity(Address address) => new(
        address.Street,
        address.City,
        address.PostalCode,
        address.Country,
        address.Type
    );

    public Address ToEntity() => new()
    {
        Street = Street,
        City = City,
        PostalCode = PostalCode,
        Country = Country,
        Type = Type
    };
}
