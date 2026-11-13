using BuildingBlocks.Application.Results;
using Catalog.Application.Features.Venues.Common.Layout;
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

    public static Error LayoutIncomplete(long id, IReadOnlyList<LayoutViolation> violations) =>
        Error.Conflict(
            "Venues.LayoutIncomplete",
            $"{nameof(Venue)} with id {id} has an incomplete layout and cannot host events.",
            violations
        );

    public static readonly Error LayoutLocked = Error.Conflict(
        "Venues.LayoutLocked",
        $"{nameof(Venue)} has events and its layout cannot be changed."
    );
}
