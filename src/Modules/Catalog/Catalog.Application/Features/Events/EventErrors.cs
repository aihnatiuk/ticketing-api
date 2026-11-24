using BuildingBlocks.Domain.Results;
using Catalog.Domain.Entities;
using Catalog.Domain.Enums;

namespace Catalog.Application.Features.Events;

public static class EventErrors
{
    public static Error NotFound(long id) =>
        Error.NotFound("Events.NotFound", $"{nameof(Event)} with id {id} was not found.");

    public static Error CannotDelete(long id, EventStatus status) =>
        Error.Conflict(
            "Events.CannotDelete",
            $"{nameof(Event)} with id {id} cannot be deleted because it has a status '{status}'."
        );

    public static Error NotPublishable(long id, EventStatus status) =>
        Error.Conflict(
            "Events.NotPublishable",
            $"{nameof(Event)} with id {id} cannot be published because it has a status '{status}'."
        );

    public static Error PricesIncomplete(long id, IReadOnlyList<long> pricingZoneIds) =>
        Error.Conflict(
            "Events.PricesIncomplete",
            $"{nameof(Event)} with id {id} cannot be published because some pricing zones have no prices.",
            new { PricingZoneIds = pricingZoneIds }
        );

    public static Error ConcurrentModification(long id) =>
        Error.Conflict(
            "Events.ConcurrentModification",
            $"{nameof(Event)} with id {id} was modified by another request. Retry the operation."
        );

    public static Error CannotModifyCompleted(long id) =>
        Error.Conflict(
            "Events.CannotModifyCompleted",
            $"{nameof(Event)} with id {id} cannot be modified because it was completed."
        );
}
