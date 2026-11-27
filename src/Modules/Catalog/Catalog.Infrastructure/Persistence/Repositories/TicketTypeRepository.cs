using BuildingBlocks.Infrastructure.Persistence;
using Catalog.Application.Abstractions;
using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Persistence.Repositories;

internal sealed class TicketTypeRepository(CatalogDbContext context)
    : BaseRepository<TicketType, long>(context),
        ITicketTypeRepository
{
    public Task<bool> LabelExistsAsync(
        string label,
        long? excludeTicketTypeId = null,
        CancellationToken cancellationToken = default
    ) => Entities.AnyAsync(t => t.Label == label && t.Id != excludeTicketTypeId, cancellationToken);

    public Task<bool> HasEventPricesAsync(
        long ticketTypeId,
        CancellationToken cancellationToken = default
    ) => context.EventPrices.AnyAsync(p => p.TicketTypeId == ticketTypeId, cancellationToken);
}
