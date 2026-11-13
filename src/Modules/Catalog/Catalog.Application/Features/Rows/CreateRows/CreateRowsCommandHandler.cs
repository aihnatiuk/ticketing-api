using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Results;
using Catalog.Application.Abstractions;
using Catalog.Application.Features.Sections;
using Catalog.Application.Features.Venues;
using Catalog.Domain.Entities;
using Catalog.Domain.Enums;

namespace Catalog.Application.Features.Rows.CreateRows;

internal sealed class CreateRowsCommandHandler(
    IVenueRepository venues,
    ISectionRepository sections,
    IRowRepository rows,
    IUnitOfWork<CatalogModule> unitOfWork
) : ICommandHandler<CreateRowsCommand, IReadOnlyList<long>>
{
    public async Task<Result<IReadOnlyList<long>>> HandleAsync(
        CreateRowsCommand command,
        CancellationToken cancellationToken = default
    )
    {
        Section? section = await sections.GetByIdAsync(command.SectionId, cancellationToken);
        if (section is null)
        {
            return SectionErrors.NotFound(command.SectionId);
        }

        if (await venues.HasEventsAsync(section.VenueId, cancellationToken))
        {
            return VenueErrors.LayoutLocked;
        }

        if (section.Type != SectionType.ReservedSeating)
        {
            return RowErrors.SectionNotReserved;
        }

        IReadOnlyList<string> existingLabels = await rows.GetExistingLabelsAsync(
            section.Id,
            [.. command.Rows.Select(input => input.Label)],
            cancellationToken
        );
        if (existingLabels.Count > 0)
        {
            return RowErrors.LabelTaken(existingLabels[0]);
        }

        var createdRows = new List<Row>(command.Rows.Count);
        foreach (CreateRowInput input in command.Rows)
        {
            var row = new Row { SectionId = section.Id, Label = input.Label };
            rows.Add(row);
            createdRows.Add(row);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<IReadOnlyList<long>>.Success([.. createdRows.Select(row => row.Id)]);
    }
}
