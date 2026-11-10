using BuildingBlocks.Application.Results;
using Catalog.Domain.Entities;

namespace Catalog.Application.Features.Seats;

public static class SeatErrors
{
    public static Error NotFound(long id) =>
        Error.NotFound("Seats.NotFound", $"{nameof(Seat)} with id {id} was not found.");

    public static Error LabelTaken(string label) =>
        Error.Conflict(
            "Seats.LabelTaken",
            $"{nameof(Seat)} with label '{label}' already exists in this row."
        );
}
