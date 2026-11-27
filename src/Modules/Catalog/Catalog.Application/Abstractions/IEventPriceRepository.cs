using BuildingBlocks.Application.Abstractions;
using Catalog.Domain.Entities;

namespace Catalog.Application.Abstractions;

public interface IEventPriceRepository : IRepository<EventPrice, long>
{
    Task<bool> CombinationExistsAsync(
        long eventId,
        long pricingZoneId,
        long ticketTypeId,
        CancellationToken cancellationToken = default
    );
}
