using BuildingBlocks.Application.Abstractions;

namespace Catalog.Application.Features.TicketTypes.DeleteTicketType;

public sealed record DeleteTicketTypeCommand(long Id) : ICommand;
