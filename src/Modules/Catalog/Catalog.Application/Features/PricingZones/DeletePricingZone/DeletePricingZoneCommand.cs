using BuildingBlocks.Application.Abstractions;

namespace Catalog.Application.Features.PricingZones.DeletePricingZone;

public sealed record DeletePricingZoneCommand(long VenueId, long Id) : ICommand;
