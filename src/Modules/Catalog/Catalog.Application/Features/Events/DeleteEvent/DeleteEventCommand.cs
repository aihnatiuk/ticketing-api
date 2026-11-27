using BuildingBlocks.Application.Abstractions;

namespace Catalog.Application.Features.Events.DeleteEvent;

public record DeleteEventCommand(long Id) : ICommand;
