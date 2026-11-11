using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Results;
using Catalog.Application.Abstractions;
using Catalog.Domain.Entities;
using Catalog.Domain.Enums;

namespace Catalog.Application.Features.EventPrices.DeleteEventPrice;

internal sealed class DeleteEventPriceCommandHandler(
    IEventPriceRepository eventPrices,
    IEventRepository events,
    IUnitOfWork<CatalogModule> unitOfWork
) : ICommandHandler<DeleteEventPriceCommand>
{
    public async Task<Result> Handle(
        DeleteEventPriceCommand command,
        CancellationToken cancellationToken = default
    )
    {
        EventPrice? eventPrice = await eventPrices.GetByIdAsync(command.Id, cancellationToken);
        if (eventPrice is null)
        {
            return EventPriceErrors.NotFound(command.Id);
        }

        Event? @event = await events.GetByIdAsync(eventPrice.EventId, cancellationToken);
        if (@event?.Status == EventStatus.Completed)
        {
            return EventPriceErrors.CannotDeleteFromCompletedEvent(
                eventPrice.Id,
                eventPrice.EventId
            );
        }

        eventPrices.Remove(eventPrice);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
