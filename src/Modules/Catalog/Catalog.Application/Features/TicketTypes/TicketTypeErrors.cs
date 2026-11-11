using BuildingBlocks.Application.Results;
using Catalog.Domain.Entities;

namespace Catalog.Application.Features.TicketTypes;

public static class TicketTypeErrors
{
    public static Error NotFound(long id) =>
        Error.NotFound("TicketTypes.NotFound", $"{nameof(TicketType)} with id {id} was not found.");

    public static Error LabelTaken(string label) =>
        Error.Conflict(
            "TicketTypes.LabelTaken",
            $"{nameof(TicketType)} with label '{label}' already exists."
        );

    public static readonly Error HasEventPrices = Error.Conflict(
        "TicketTypes.HasEventPrices",
        $"{nameof(TicketType)} is used by event prices and cannot be deleted."
    );
}
