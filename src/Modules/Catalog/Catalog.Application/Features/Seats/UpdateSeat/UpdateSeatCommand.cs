using BuildingBlocks.Application.Abstractions;

namespace Catalog.Application.Features.Seats.UpdateSeat;

public sealed record UpdateSeatCommand(long Id, string Label) : ICommand;
