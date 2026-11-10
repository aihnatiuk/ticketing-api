using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Results;
using Catalog.Application.Abstractions;
using Catalog.Application.Features.Venues;
using Catalog.Domain.Entities;

namespace Catalog.Application.Features.Seats.UpdateSeat;

internal sealed class UpdateSeatCommandHandler(
    IVenueRepository venues,
    ISeatRepository seats,
    IUnitOfWork<CatalogModule> unitOfWork
) : ICommandHandler<UpdateSeatCommand>
{
    public async Task<Result> Handle(
        UpdateSeatCommand command,
        CancellationToken cancellationToken = default
    )
    {
        Seat? seat = await seats.GetByIdWithRowAndSectionAsync(command.Id, cancellationToken);
        if (seat is null)
        {
            return SeatErrors.NotFound(command.Id);
        }

        if (await venues.HasEventsAsync(seat.Row!.Section!.VenueId, cancellationToken))
        {
            return VenueErrors.LayoutLocked;
        }

        if (await seats.LabelExistsAsync(seat.RowId, command.Label, seat.Id, cancellationToken))
        {
            return SeatErrors.LabelTaken(command.Label);
        }

        seat.Label = command.Label;
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
