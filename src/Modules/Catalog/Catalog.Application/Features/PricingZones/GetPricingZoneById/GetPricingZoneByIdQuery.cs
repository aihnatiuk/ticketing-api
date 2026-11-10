using BuildingBlocks.Application.Abstractions;
using Catalog.Application.Features.PricingZones.Common;

namespace Catalog.Application.Features.PricingZones.GetPricingZoneById;

public sealed record GetPricingZoneByIdQuery(long VenueId, long Id) : IQuery<PricingZoneDto>;
