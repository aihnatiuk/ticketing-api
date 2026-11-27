using BuildingBlocks.Infrastructure.Persistence;
using Catalog.Application.Abstractions;
using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Persistence.Repositories;

internal sealed class PricingZoneRepository(CatalogDbContext context)
    : BaseRepository<PricingZone, long>(context),
        IPricingZoneRepository
{
    public Task<bool> LabelExistsAsync(
        long venueId,
        string label,
        long? excludeZoneId = null,
        CancellationToken cancellationToken = default
    ) =>
        Entities.AnyAsync(
            z => z.VenueId == venueId && z.Label == label && z.Id != excludeZoneId,
            cancellationToken
        );

    public Task<bool> HasSectionsAsync(
        long zoneId,
        CancellationToken cancellationToken = default
    ) => context.Sections.AnyAsync(s => s.PricingZoneId == zoneId, cancellationToken);

    public Task<bool> HasEventPricesAsync(
        long zoneId,
        CancellationToken cancellationToken = default
    ) => context.EventPrices.AnyAsync(p => p.PricingZoneId == zoneId, cancellationToken);
}
