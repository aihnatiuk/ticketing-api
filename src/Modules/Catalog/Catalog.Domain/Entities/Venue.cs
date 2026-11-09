using BuildingBlocks.Domain;
using Catalog.Domain.ValueObjects;

namespace Catalog.Domain.Entities;

public sealed class Venue : Entity<long>
{
    public required string Name { get; set; }

    public required Address Address { get; set; }

    public ICollection<Section> Sections { get; set; } = [];

    public ICollection<PricingZone> PricingZones { get; set; } = [];
}
