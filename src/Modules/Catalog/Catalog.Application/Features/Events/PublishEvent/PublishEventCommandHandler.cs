using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Domain.Results;
using Catalog.Application.Abstractions;
using Catalog.Application.Features.Venues;
using Catalog.Application.Features.Venues.Common.Layout;
using Catalog.Contracts.IntegrationEvents;
using Catalog.Domain.Entities;
using Catalog.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Events.PublishEvent;

internal sealed class PublishEventCommandHandler(
    IEventRepository events,
    IVenueLayoutValidator layoutValidator,
    IEventPublishingReader publishingReader,
    IOutbox<CatalogModule> outbox,
    IUnitOfWork<CatalogModule> unitOfWork
) : ICommandHandler<PublishEventCommand>
{
    public async Task<Result> HandleAsync(
        PublishEventCommand command,
        CancellationToken cancellationToken = default
    )
    {
        Event? @event = await events.GetByIdAsync(command.EventId, cancellationToken);
        if (@event is null)
        {
            return EventErrors.NotFound(command.EventId);
        }

        if (@event.Status is not EventStatus.Created)
        {
            return EventErrors.NotPublishable(@event.Id, @event.Status);
        }

        IReadOnlyList<LayoutViolation> violations = await layoutValidator.ValidateAsync(
            @event.VenueId,
            cancellationToken: cancellationToken
        );
        if (violations.Count > 0)
        {
            return VenueErrors.LayoutIncomplete(@event.VenueId, violations);
        }

        IReadOnlyList<long> unpricedZoneIds = await publishingReader.GetZonesWithoutPricesAsync(
            @event.Id,
            @event.VenueId,
            cancellationToken
        );
        if (unpricedZoneIds.Count > 0)
        {
            return EventErrors.PricesIncomplete(@event.Id, unpricedZoneIds);
        }

        EventAvailabilitySnapshot availability =
            await publishingReader.GetAvailabilitySnapshotAsync(@event.VenueId, cancellationToken);
        if (availability.IsEmpty)
        {
            // A venue that passed layout validation has at least one section, and its layout is locked
            // once events exist, so an empty snapshot means the data is corrupted or business rules were broken.
            throw new InvalidOperationException(
                $"Venue {@event.VenueId} passed layout validation but has no seats or general admission sections."
            );
        }

        @event.Publish();
        outbox.Add(
            new EventPublishedIntegrationEvent(
                @event.Id,
                availability.SeatIds,
                availability
                    .GeneralAdmissionSections.Select(s => new GeneralAdmissionSectionDto(
                        s.SectionId,
                        s.Capacity
                    ))
                    .ToList()
            )
        );

        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            return EventErrors.ConcurrentModification(@event.Id);
        }

        return Result.Success();
    }
}
