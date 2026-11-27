using BuildingBlocks.Application.Abstractions;

namespace Catalog.Application.Features.Events.PublishEvent;

public record PublishEventCommand(long EventId) : ICommand;
