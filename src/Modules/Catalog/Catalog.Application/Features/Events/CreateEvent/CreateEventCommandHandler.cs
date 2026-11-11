using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Results;
using Catalog.Application.Abstractions;
using Catalog.Application.Features.Venues;
using Catalog.Application.Features.Venues.Common.Layout;
using Catalog.Domain.Entities;

namespace Catalog.Application.Features.Events.CreateEvent;

internal sealed class CreateEventCommandHandler(
    IEventRepository events,
    IVenueRepository venues,
    IVenueLayoutValidator layoutValidator,
    IUnitOfWork<CatalogModule> unitOfWork
) : ICommandHandler<CreateEventCommand, long>
{
    public async Task<Result<long>> Handle(
        CreateEventCommand command,
        CancellationToken cancellationToken = default
    )
    {
        if (!await venues.ExistsAsync(command.VenueId, cancellationToken))
        {
            return VenueErrors.NotFound(command.VenueId);
        }

        IReadOnlyList<LayoutViolation> violations = await layoutValidator.ValidateAsync(
            command.VenueId,
            cancellationToken: cancellationToken
        );
        if (violations.Count > 0)
        {
            return VenueErrors.LayoutIncomplete(command.VenueId, violations);
        }

        Event @event = new Event
        {
            Title = command.Title,
            Description = command.Description,
            StartsAt = command.StartsAt,
            EndsAt = command.EndsAt,
            VenueId = command.VenueId,
            Currency = command.Currency,
        };

        events.Add(@event);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return @event.Id;
    }
}
