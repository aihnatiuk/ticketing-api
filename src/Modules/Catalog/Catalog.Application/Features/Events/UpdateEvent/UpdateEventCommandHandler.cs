using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Results;
using Catalog.Application.Abstractions;
using Catalog.Domain.Entities;
using Catalog.Domain.Enums;

namespace Catalog.Application.Features.Events.UpdateEvent;

internal sealed class UpdateEventCommandHandler(
    IEventRepository events,
    IUnitOfWork<CatalogModule> unitOfWork
) : ICommandHandler<UpdateEventCommand>
{
    public async Task<Result> Handle(
        UpdateEventCommand command,
        CancellationToken cancellationToken = default
    )
    {
        Event? @event = await events.GetByIdAsync(command.Id, cancellationToken);
        if (@event is null)
        {
            return EventErrors.NotFound(command.Id);
        }

        if (@event.Status is EventStatus.Completed)
        {
            return EventErrors.CannotModifyCompleted(@event.Id);
        }

        @event.Title = command.Title;
        @event.Description = command.Description;
        @event.StartsAt = command.StartsAt;
        @event.EndsAt = command.EndsAt;
        @event.Currency = command.Currency;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
