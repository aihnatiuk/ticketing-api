using BuildingBlocks.Domain.Results;
using Catalog.Domain.Entities;

namespace Catalog.Application.Features.Seats;

public static class SeatErrors
{
    public static Error NotFound(long id) =>
        Error.NotFound("Seats.NotFound", $"{nameof(Seat)} with id {id} was not found.");

    public static Error NotFoundForEvent(long eventId, long[] missingSeatsIds) =>
        Error.NotFound(
            "Seats.NotFoundForEvent",
            $"Some seats are not related to the event with id {eventId}.",
            missingSeatsIds
        );

    public static Error LabelTaken(string label) =>
        Error.Conflict(
            "Seats.LabelTaken",
            $"{nameof(Seat)} with label '{label}' already exists in this row."
        );
}
