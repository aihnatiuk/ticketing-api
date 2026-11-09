using BuildingBlocks.Application.Abstractions;
using Catalog.Domain.Entities;

namespace Catalog.Application.Abstractions;

public interface IVenueRepository : IRepository<Venue, long>
{
    Task<bool> HasEventsAsync(long venueId, CancellationToken cancellationToken = default);
}
