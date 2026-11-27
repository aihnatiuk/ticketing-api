using BuildingBlocks.Application.Abstractions;
using Catalog.Application.Features.PricingZones.Common;

namespace Catalog.Application.Features.PricingZones.GetPricingZones;

public sealed record GetPricingZonesQuery(long VenueId) : IQuery<IReadOnlyList<PricingZoneDto>>;
