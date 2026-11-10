using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Results;
using Catalog.Application.Abstractions;
using Catalog.Application.Features.Venues;
using Catalog.Domain.Entities;

namespace Catalog.Application.Features.Seats.DeleteSeat;

internal sealed class DeleteSeatCommandHandler(
    IVenueRepository venues,
    ISeatRepository seats,
    IUnitOfWork<CatalogModule> unitOfWork
) : ICommandHandler<DeleteSeatCommand>
{
    public async Task<Result> Handle(
        DeleteSeatCommand command,
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

        seats.Remove(seat);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
