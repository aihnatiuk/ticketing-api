using BuildingBlocks.Infrastructure.Persistence;
using Catalog.Application.Abstractions;
using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Persistence.Repositories;

internal sealed class VenueRepository(CatalogDbContext context)
    : BaseRepository<Venue, long>(context),
        IVenueRepository
{
    public Task<bool> ExistsAsync(long venueId, CancellationToken cancellationToken = default) =>
        Entities.AnyAsync(v => v.Id == venueId, cancellationToken);

    public Task<bool> HasEventsAsync(long venueId, CancellationToken cancellationToken = default) =>
        context.Events.AnyAsync(e => e.VenueId == venueId, cancellationToken);
}
