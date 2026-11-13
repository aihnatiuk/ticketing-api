using BuildingBlocks.Application.Abstractions;

namespace Catalog.Application.Features.Seats.CreateSeats;

public sealed record CreateSeatInput(string Label);

public sealed record CreateSeatsCommand(long RowId, IReadOnlyList<CreateSeatInput> Seats)
    : ICommand<IReadOnlyList<long>>;
