using BuildingBlocks.Application.Results;
using Catalog.Domain.Entities;

namespace Catalog.Application.Features.Sections;

public static class SectionErrors
{
    public static Error NotFound(long id) =>
        Error.NotFound("Sections.NotFound", $"{nameof(Section)} with id {id} was not found.");

    public static Error NotFoundInVenue(long id, long venueId) =>
        Error.NotFound(
            "Sections.NotFoundInVenue",
            $"{nameof(Section)} with id {id} was not found or doesn't belong to the venue with id {venueId}."
        );

    public static Error LabelTaken(string label) =>
        Error.Conflict(
            "Sections.LabelTaken",
            $"{nameof(Section)} with label '{label}' already exists in this venue."
        );
}
