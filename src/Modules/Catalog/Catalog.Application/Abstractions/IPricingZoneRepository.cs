using BuildingBlocks.Application.Abstractions;
using Catalog.Domain.Entities;

namespace Catalog.Application.Abstractions;

public interface IPricingZoneRepository : IRepository<PricingZone, long>
{
    /// <summary>
    /// Checks whether a pricing zone with the given label already exists in the venue.
    /// <paramref name="venueId"/>The Id of the venue to check.</paramref>
    /// <paramref name="label"/>The label to check.</paramref>
    /// <paramref name="excludeZoneId"/>An Id of a zone to exclude from the check. Can be used when updating a zone's Label to the same value with different casing. If null, all zones will be checked.</paramref>
    /// </summary>
    Task<bool> LabelExistsAsync(
        long venueId,
        string label,
        long? excludeZoneId = null,
        CancellationToken cancellationToken = default
    );

    Task<bool> HasSectionsAsync(long zoneId, CancellationToken cancellationToken = default);

    Task<bool> HasEventPricesAsync(long zoneId, CancellationToken cancellationToken = default);
}
