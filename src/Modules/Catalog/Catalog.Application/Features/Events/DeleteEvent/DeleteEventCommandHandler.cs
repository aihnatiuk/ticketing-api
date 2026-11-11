using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Results;
using Catalog.Application.Abstractions;
using Catalog.Domain.Entities;
using Catalog.Domain.Enums;

namespace Catalog.Application.Features.Events.DeleteEvent;

internal sealed class DeleteEventCommandHandler(
    IEventRepository events,
    IUnitOfWork<CatalogModule> unitOfWork
) : ICommandHandler<DeleteEventCommand>
{
    public async Task<Result> Handle(
        DeleteEventCommand command,
        CancellationToken cancellationToken = default
    )
    {
        Event? @event = await events.GetByIdAsync(command.Id, cancellationToken);
        if (@event is null)
        {
            return EventErrors.NotFound(command.Id);
        }

        if (@event.Status != EventStatus.Created)
        {
            return EventErrors.CannotDelete(@event.Id, @event.Status);
        }

        events.Remove(@event);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
