using BuildingBlocks.Domain;
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

    public EventStatus Status { get; set; } = EventStatus.Created;
}
