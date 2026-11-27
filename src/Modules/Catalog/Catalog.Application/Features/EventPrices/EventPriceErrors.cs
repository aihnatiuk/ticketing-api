using BuildingBlocks.Application.Results;
using Catalog.Domain.Entities;

namespace Catalog.Application.Features.EventPrices;

public static class EventPriceErrors
{
    public static Error NotFound(long id) =>
        Error.NotFound("EventPrices.NotFound", $"{nameof(EventPrice)} with id {id} was not found.");

    public static Error CannotDeleteFromCompletedEvent(long id, long eventId) =>
        Error.Conflict(
            "EventPrices.CannotDeleteFromCompletedEvent",
            $"{nameof(EventPrice)} with id {id} cannot be deleted because event {eventId} is completed."
        );

    public static Error CannotUpdateFromCompletedEvent(long id, long eventId) =>
        Error.Conflict(
            "EventPrices.CannotUpdateFromCompletedEvent",
            $"{nameof(EventPrice)} with id {id} cannot be updated because event {eventId} is completed."
        );

    public static Error AlreadyExists(long eventId, long pricingZoneId, long ticketTypeId) =>
        Error.Conflict(
            "EventPrices.AlreadyExists",
            $"{nameof(EventPrice)} for event {eventId}, pricing zone {pricingZoneId} and ticket type {ticketTypeId} already exists."
        );
}
