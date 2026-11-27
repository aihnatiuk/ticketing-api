using BuildingBlocks.Application.Abstractions;

namespace Catalog.Application.Features.PricingZones.UpdatePricingZone;

public sealed record UpdatePricingZoneCommand(long VenueId, long Id, string Label) : ICommand;
