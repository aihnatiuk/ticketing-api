using BuildingBlocks.Domain;
using BuildingBlocks.Domain.Exceptions;
using Catalog.Domain.Enums;

namespace Catalog.Domain.Entities;

public sealed class Event : Entity<long>
{
    public required string Title { get; set; }

    public string? Description { get; set; }

    public required DateTimeOffset StartsAt { get; set; }

    public required DateTimeOffset EndsAt { get; set; }

    public required long VenueId { get; set; }

    public Venue? Venue { get; set; }

    public required string Currency { get; set; }

    public ICollection<EventPrice> Prices { get; set; } = [];

    public EventStatus Status { get; private set; } = EventStatus.Created;

    /// <summary>
    /// Makes the event available for booking.
    /// </summary>
    /// <exception cref="DomainInvariantException">The event is not in the <see cref="EventStatus.Created"/> status.</exception>
    public void Publish()
    {
        if (Status is not EventStatus.Created)
        {
            throw new DomainInvariantException(
                $"Only an event with the status '{EventStatus.Created}' can be published, but it is '{Status}'."
            );
        }

        Status = EventStatus.Published;
    }
}
