using BuildingBlocks.Application.Results;
using Catalog.Domain.Entities;

namespace Catalog.Application.Features.Venues;

public static class VenueErrors
{
    public static Error NotFound(long id) =>
        Error.NotFound("Venues.NotFound", $"{nameof(Venue)} with id {id} was not found.");

    public static readonly Error HasEvents = Error.Conflict(
        "Venues.HasEvents",
        $"{nameof(Venue)} has events and cannot be deleted."
    );

    public static readonly Error LayoutLocked = Error.Conflict(
        "Venues.LayoutLocked",
        $"{nameof(Venue)} has events and its layout cannot be changed."
    );
}
