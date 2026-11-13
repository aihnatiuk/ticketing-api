using Catalog.Domain.Entities;

namespace Catalog.Application.Features.PricingZones.Common;

public sealed record PricingZoneDto(long Id, long VenueId, string Label)
{
    internal static PricingZoneDto FromDomain(PricingZone zone) =>
        new(zone.Id, zone.VenueId, zone.Label);
}
