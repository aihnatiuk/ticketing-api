using BuildingBlocks.Domain;

namespace Catalog.Domain.Entities;

public sealed class PricingZone : Entity<long>
{
    public required string Label { get; set; }

    public required long VenueId { get; set; }
}
