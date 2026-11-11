using BuildingBlocks.Application.Abstractions;

namespace Catalog.Application.Features.TicketTypes.UpdateTicketType;

public sealed record UpdateTicketTypeCommand(long Id, string Label) : ICommand;
