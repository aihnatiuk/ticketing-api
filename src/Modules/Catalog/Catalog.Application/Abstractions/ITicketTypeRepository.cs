using BuildingBlocks.Application.Abstractions;
using Catalog.Domain.Entities;

namespace Catalog.Application.Abstractions;

public interface ITicketTypeRepository : IRepository<TicketType, long>
{
    /// <summary>
    /// Checks whether a ticket type with the given label already exists.
    /// <paramref name="label"/>The label to check.</paramref>
    /// <paramref name="excludeTicketTypeId"/>An Id of a ticket type to exclude from the check. Can be used when updating a ticket type to its current label. If null, all ticket types will be checked.</paramref>
    /// </summary>
    Task<bool> LabelExistsAsync(
        string label,
        long? excludeTicketTypeId = null,
        CancellationToken cancellationToken = default
    );

    Task<bool> HasEventPricesAsync(
        long ticketTypeId,
        CancellationToken cancellationToken = default
    );
}
