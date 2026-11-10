using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Results;
using Catalog.Application.Abstractions;
using Catalog.Application.Features.Venues;
using Catalog.Domain.Entities;

namespace Catalog.Application.Features.Rows.DeleteRow;

internal sealed class DeleteRowCommandHandler(
    IVenueRepository venues,
    IRowRepository rows,
    IUnitOfWork<CatalogModule> unitOfWork
) : ICommandHandler<DeleteRowCommand>
{
    public async Task<Result> Handle(
        DeleteRowCommand command,
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

        rows.Remove(row);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
