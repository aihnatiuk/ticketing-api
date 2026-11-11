using BuildingBlocks.Application.Results;
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

    public static Error CannotModifyCompleted(long id) =>
        Error.Conflict(
            "Events.CannotModifyCompleted",
            $"{nameof(Event)} with id {id} cannot be modified because it was completed."
        );
}
