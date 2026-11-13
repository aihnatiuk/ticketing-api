using BuildingBlocks.Application.Abstractions;

namespace Catalog.Application.Features.PricingZones.CreatePricingZone;

public sealed record CreatePricingZoneCommand(long VenueId, string Label) : ICommand<long>;
