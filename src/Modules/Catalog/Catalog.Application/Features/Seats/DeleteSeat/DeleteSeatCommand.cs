using BuildingBlocks.Application.Abstractions;

namespace Catalog.Application.Features.Seats.DeleteSeat;

public sealed record DeleteSeatCommand(long Id) : ICommand;
