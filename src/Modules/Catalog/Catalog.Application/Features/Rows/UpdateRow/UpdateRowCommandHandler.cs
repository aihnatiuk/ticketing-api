using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Results;
using Catalog.Application.Abstractions;
using Catalog.Application.Features.Venues;
using Catalog.Domain.Entities;

namespace Catalog.Application.Features.Rows.UpdateRow;

internal sealed class UpdateRowCommandHandler(
    IVenueRepository venues,
    IRowRepository rows,
    IUnitOfWork<CatalogModule> unitOfWork
) : ICommandHandler<UpdateRowCommand>
{
    public async Task<Result> HandleAsync(
        UpdateRowCommand command,
        CancellationToken cancellationToken = default
    )
    {
        Row? row = await rows.GetByIdWithSectionAsync(command.Id, cancellationToken);
        if (row is null)
        {
            return RowErrors.NotFound(command.Id);
        }

        if (await venues.HasEventsAsync(row.Section!.VenueId, cancellationToken))
        {
            return VenueErrors.LayoutLocked;
        }

        if (await rows.LabelExistsAsync(row.SectionId, command.Label, row.Id, cancellationToken))
        {
            return RowErrors.LabelTaken(command.Label);
        }

        row.Label = command.Label;
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
