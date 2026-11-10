using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Results;
using Catalog.Application.Abstractions;
using Catalog.Application.Features.PricingZones.Common;
using Catalog.Application.Features.Venues;
using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.PricingZones.GetPricingZones;

internal sealed class GetPricingZonesQueryHandler(ICatalogDbContext db)
    : IQueryHandler<GetPricingZonesQuery, IReadOnlyList<PricingZoneDto>>
{
    public async Task<Result<IReadOnlyList<PricingZoneDto>>> Handle(
        GetPricingZonesQuery query,
        CancellationToken cancellationToken = default
    )
    {
        if (!await db.Venues.AnyAsync(v => v.Id == query.VenueId, cancellationToken))
        {
            return VenueErrors.NotFound(query.VenueId);
        }

        List<PricingZone> zones = await db
            .PricingZones.AsNoTracking()
            .Where(z => z.VenueId == query.VenueId)
            .OrderBy(z => z.Label)
            .ToListAsync(cancellationToken);

        IReadOnlyList<PricingZoneDto> items = [.. zones.Select(PricingZoneDto.FromDomain)];

        return Result<IReadOnlyList<PricingZoneDto>>.Success(items);
    }
}
