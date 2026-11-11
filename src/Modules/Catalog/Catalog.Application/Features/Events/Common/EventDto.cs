using Catalog.Domain.Entities;
using Catalog.Domain.Enums;

namespace Catalog.Application.Features.Events.Common;

public sealed record class EventDto(
    long Id,
    string Title,
    string? Description,
    DateTimeOffset StartsAt,
    DateTimeOffset EndsAt,
    long VenueId,
    string Currency,
    EventStatus Status
)
{
    internal static EventDto FromDomain(Event @event) =>
        new(
            @event.Id,
            @event.Title,
            @event.Description,
            @event.StartsAt,
            @event.EndsAt,
            @event.VenueId,
            @event.Currency,
            @event.Status
        );
}
