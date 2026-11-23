namespace Catalog.Application.Features.Events.PublishEvent;

internal interface IEventPublishingReader
{
    /// <summary>
    /// Finds the pricing zones used by the event's venue that have no price defined for the event.
    /// </summary>
    /// <returns>Ids of the zones missing prices; empty when every zone is priced.</returns>
    Task<IReadOnlyList<long>> GetZonesWithoutPricesAsync(
        long eventId,
        long venueId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Collects the seats and general-admission capacities offered by the venue.
    /// </summary>
    Task<EventAvailabilitySnapshot> GetAvailabilitySnapshotAsync(
        long venueId,
        CancellationToken cancellationToken = default
    );
}
