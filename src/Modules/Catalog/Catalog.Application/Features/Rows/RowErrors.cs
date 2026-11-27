using BuildingBlocks.Application.Results;
using Catalog.Domain.Entities;

namespace Catalog.Application.Features.Rows;

public static class RowErrors
{
    public static Error NotFound(long id) =>
        Error.NotFound("Rows.NotFound", $"{nameof(Row)} with id {id} was not found.");

    public static Error LabelTaken(string label) =>
        Error.Conflict(
            "Rows.LabelTaken",
            $"{nameof(Row)} with label '{label}' already exists in this section."
        );

    public static readonly Error SectionNotReserved = Error.Conflict(
        "Rows.SectionNotReserved",
        $"{nameof(Row)} can only be added to a reserved-seating section."
    );
}
