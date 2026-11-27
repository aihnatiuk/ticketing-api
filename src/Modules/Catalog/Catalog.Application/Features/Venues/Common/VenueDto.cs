using Catalog.Domain.Entities;

namespace Catalog.Application.Features.Venues.Common;

public sealed record VenueDto(long Id, string Name, AddressDto Address)
{
    internal static VenueDto FromDomain(Venue venue) =>
        new(venue.Id, venue.Name, AddressDto.FromDomain(venue.Address));
}
