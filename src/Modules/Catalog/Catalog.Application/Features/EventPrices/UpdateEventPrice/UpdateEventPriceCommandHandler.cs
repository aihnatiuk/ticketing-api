using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Results;
using Catalog.Application.Abstractions;
using Catalog.Domain.Entities;
using Catalog.Domain.Enums;

namespace Catalog.Application.Features.EventPrices.UpdateEventPrice;

internal sealed class UpdateEventPriceCommandHandler(
    IEventPriceRepository eventPrices,
    IEventRepository events,
    IUnitOfWork<CatalogModule> unitOfWork
) : ICommandHandler<UpdateEventPriceCommand>
{
    public async Task<Result> Handle(
        UpdateEventPriceCommand command,
        CancellationToken cancellationToken = default
    )
    {
        EventPrice? eventPrice = await eventPrices.GetByIdAsync(command.Id, cancellationToken);
        if (eventPrice is null)
        {
            return EventPriceErrors.NotFound(command.Id);
        }

        Event? @event = await events.GetByIdAsync(eventPrice.EventId, cancellationToken);
        if (@event is null)
        {
            // The owning event has been deleted between the calls, so EventPrice shouldn't exist anymore.
            return EventPriceErrors.NotFound(command.Id);
        }

        if (@event.Status == EventStatus.Completed)
        {
            return EventPriceErrors.CannotUpdateFromCompletedEvent(
                eventPrice.Id,
                eventPrice.EventId
            );
        }

        eventPrice.Amount = command.Amount;
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
