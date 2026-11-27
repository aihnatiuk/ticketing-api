using BuildingBlocks.Infrastructure.Persistence;
using Catalog.Application.Abstractions;
using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Persistence.Repositories;

internal sealed class EventPriceRepository(CatalogDbContext context)
    : BaseRepository<EventPrice, long>(context),
        IEventPriceRepository
{
    public Task<bool> CombinationExistsAsync(
        long eventId,
        long pricingZoneId,
        long ticketTypeId,
        CancellationToken cancellationToken = default
    ) =>
        Entities.AnyAsync(
            p =>
                p.EventId == eventId
                && p.PricingZoneId == pricingZoneId
                && p.TicketTypeId == ticketTypeId,
            cancellationToken
        );
}
