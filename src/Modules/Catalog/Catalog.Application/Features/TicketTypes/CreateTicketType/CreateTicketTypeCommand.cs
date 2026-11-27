using BuildingBlocks.Application.Abstractions;

namespace Catalog.Application.Features.TicketTypes.CreateTicketType;

public sealed record CreateTicketTypeCommand(string Label) : ICommand<long>;
