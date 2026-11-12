using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Results;
using Catalog.Application.Abstractions;
using Catalog.Application.Features.Rows;
using Catalog.Application.Features.Venues;
using Catalog.Domain.Entities;

namespace Catalog.Application.Features.Seats.CreateSeats;

internal sealed class CreateSeatsCommandHandler(
    IVenueRepository venues,
    IRowRepository rows,
    ISeatRepository seats,
    IUnitOfWork<CatalogModule> unitOfWork
) : ICommandHandler<CreateSeatsCommand, IReadOnlyList<long>>
{
    public async Task<Result<IReadOnlyList<long>>> HandleAsync(
        CreateSeatsCommand command,
        CancellationToken cancellationToken = default
    )
    {
        Row? row = await rows.GetByIdWithSectionAsync(command.RowId, cancellationToken);
        if (row is null)
        {
            return RowErrors.NotFound(command.RowId);
        }

        if (await venues.HasEventsAsync(row.Section!.VenueId, cancellationToken))
        {
            return VenueErrors.LayoutLocked;
        }

        IReadOnlyList<string> existingLabels = await seats.GetExistingLabelsAsync(
            row.Id,
            [.. command.Seats.Select(input => input.Label)],
            cancellationToken
        );
        if (existingLabels.Count > 0)
        {
            return SeatErrors.LabelTaken(existingLabels[0]);
        }

        var createdSeats = new List<Seat>(command.Seats.Count);
        foreach (CreateSeatInput input in command.Seats)
        {
            var seat = new Seat { RowId = row.Id, Label = input.Label };
            seats.Add(seat);
            createdSeats.Add(seat);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<IReadOnlyList<long>>.Success([.. createdSeats.Select(seat => seat.Id)]);
    }
}
