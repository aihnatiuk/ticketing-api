using BuildingBlocks.Application.Abstractions;

namespace Catalog.Application.Features.Events.CreateEvent;

public sealed record CreateEventCommand(
    string Title,
    string? Description,
    DateTimeOffset StartsAt,
    DateTimeOffset EndsAt,
    long VenueId,
    string Currency
) : ICommand<long>;
