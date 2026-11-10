using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Results;
using Catalog.Application.Abstractions;
using Catalog.Application.Features.PricingZones.Common;
using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.PricingZones.GetPricingZoneById;

internal sealed class GetPricingZoneByIdQueryHandler(ICatalogDbContext db)
    : IQueryHandler<GetPricingZoneByIdQuery, PricingZoneDto>
{
    public async Task<Result<PricingZoneDto>> Handle(
        GetPricingZoneByIdQuery query,
        CancellationToken cancellationToken = default
    )
    {
        PricingZone? zone = await db
            .PricingZones.AsNoTracking()
            .FirstOrDefaultAsync(
                z => z.Id == query.Id && z.VenueId == query.VenueId,
                cancellationToken
            );

        if (zone is null)
        {
            return PricingZoneErrors.NotFound(query.Id);
        }

        return PricingZoneDto.FromDomain(zone);
    }
}
