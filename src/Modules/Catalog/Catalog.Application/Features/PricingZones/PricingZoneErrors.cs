using BuildingBlocks.Application.Results;
using Catalog.Domain.Entities;

namespace Catalog.Application.Features.PricingZones;

public static class PricingZoneErrors
{
    public static Error NotFound(long id) =>
        Error.NotFound(
            "PricingZones.NotFound",
            $"{nameof(PricingZone)} with id {id} was not found."
        );

    public static Error LabelTaken(string label) =>
        Error.Conflict(
            "PricingZones.LabelTaken",
            $"{nameof(PricingZone)} with label '{label}' already exists in this venue."
        );

    public static readonly Error HasSections = Error.Conflict(
        "PricingZones.HasSections",
        $"{nameof(PricingZone)} is used by sections and cannot be deleted."
    );

    public static readonly Error HasEventPrices = Error.Conflict(
        "PricingZones.HasEventPrices",
        $"{nameof(PricingZone)} is used by event prices and cannot be deleted."
    );

    public static Error VenueMismatch(long id, long venueId) =>
        Error.Conflict(
            "PricingZones.VenueMismatch",
            $"{nameof(PricingZone)} {id} doesn't belong to the venue {venueId}."
        );
}
