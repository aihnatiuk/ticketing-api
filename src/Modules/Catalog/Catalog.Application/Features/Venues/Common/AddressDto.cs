using Catalog.Domain.ValueObjects;

namespace Catalog.Application.Features.Venues.Common;

public sealed record AddressDto(string Street, string City, string CountryCode)
{
    internal static AddressDto FromDomain(Address address) =>
        new(address.Street, address.City, address.CountryCode);

    internal Address ToDomain() => new(Street, City, CountryCode);
}
