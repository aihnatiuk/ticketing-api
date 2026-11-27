using BuildingBlocks.Application.Abstractions;

namespace Catalog.Application.Features.Events.UpdateEvent;

public sealed record UpdateEventCommand(
    long Id,
    string Title,
    string? Description,
    DateTimeOffset StartsAt,
    DateTimeOffset EndsAt,
    string Currency
) : ICommand;
