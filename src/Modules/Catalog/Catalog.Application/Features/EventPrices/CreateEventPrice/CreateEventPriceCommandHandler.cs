using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Results;
using Catalog.Application.Abstractions;
using Catalog.Application.Features.Events;
using Catalog.Application.Features.PricingZones;
using Catalog.Application.Features.TicketTypes;
using Catalog.Domain.Entities;

namespace Catalog.Application.Features.EventPrices.CreateEventPrice;

internal sealed class CreateEventPriceCommandHandler(
    IEventRepository events,
    IPricingZoneRepository pricingZones,
    ITicketTypeRepository ticketTypes,
    IEventPriceRepository eventPrices,
    IUnitOfWork<CatalogModule> unitOfWork
) : ICommandHandler<CreateEventPriceCommand, long>
{
    public async Task<Result<long>> Handle(
        CreateEventPriceCommand command,
        CancellationToken cancellationToken = default
    )
    {
        Event? @event = await events.GetByIdAsync(command.EventId, cancellationToken);
        if (@event is null)
        {
            return EventErrors.NotFound(command.EventId);
        }

        PricingZone? zone = await pricingZones.GetByIdAsync(
            command.PricingZoneId,
            cancellationToken
        );
        if (zone is null)
        {
            return PricingZoneErrors.NotFound(command.PricingZoneId);
        }

        if (zone.VenueId != @event.VenueId)
        {
            return PricingZoneErrors.VenueMismatch(zone.Id, @event.VenueId);
        }

        TicketType? ticketType = await ticketTypes.GetByIdAsync(
            command.TicketTypeId,
            cancellationToken
        );
        if (ticketType is null)
        {
            return TicketTypeErrors.NotFound(command.TicketTypeId);
        }

        if (
            await eventPrices.CombinationExistsAsync(
                command.EventId,
                command.PricingZoneId,
                command.TicketTypeId,
                cancellationToken
            )
        )
        {
            return EventPriceErrors.AlreadyExists(
                command.EventId,
                command.PricingZoneId,
                command.TicketTypeId
            );
        }

        var eventPrice = new EventPrice
        {
            EventId = command.EventId,
            PricingZoneId = command.PricingZoneId,
            TicketTypeId = command.TicketTypeId,
            Amount = command.Amount,
        };

        eventPrices.Add(eventPrice);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return eventPrice.Id;
    }
}
