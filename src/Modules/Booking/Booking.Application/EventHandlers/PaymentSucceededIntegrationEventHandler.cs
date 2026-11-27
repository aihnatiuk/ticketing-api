using Booking.Application.Abstractions;
using Booking.Domain.Entities;
using Booking.Domain.Enums;
using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Messaging;
using Payment.Contracts.IntegrationEvents;

namespace Booking.Application.EventHandlers;

/// <summary>
/// Marks the order as paid and sells its dedicated seats once the payment has succeeded.
/// </summary>
internal sealed class PaymentSucceededIntegrationEventHandler(
    IOrderRepository orders,
    ISeatRepository seats,
    IUnitOfWork<BookingModule> unitOfWork
) : IIntegrationEventHandler<PaymentSucceededIntegrationEvent>
{
    public async Task HandleAsync(
        PaymentSucceededIntegrationEvent integrationEvent,
        CancellationToken cancellationToken = default
    )
    {
        Order? order = await orders.GetWithItemsAsync(integrationEvent.OrderId, cancellationToken);
        if (order is null)
        {
            throw new InvalidOperationException(
                $"Payment {integrationEvent.PaymentId} succeeded for order {integrationEvent.OrderId}, which does not exist."
            );
        }

        if (order.Status == OrderStatus.Paid && order.PaymentId == integrationEvent.PaymentId)
        {
            return;
        }

        // Money was taken for an order that cannot be fulfilled
        if (
            order.Status != OrderStatus.PaymentProcessing
            || order.PaymentId != integrationEvent.PaymentId
        )
        {
            throw new InvalidOperationException(
                $"Payment {integrationEvent.PaymentId} succeeded, but order {order.Id} is {order.Status} with payment {order.PaymentId}."
            );
        }

        long[] seatIds =
        [
            .. order
                .OrderItems.Where(i => i.Type == OrderItemType.DedicatedSeat)
                .Select(i => i.CatalogRefId),
        ];

        DedicatedSeat[] dedicatedSeats = await seats.GetDedicatedSeatsAsync(
            order.EventId,
            seatIds,
            cancellationToken
        );
        foreach (DedicatedSeat seat in dedicatedSeats)
        {
            seat.Sell();
        }

        order.MarkPaid();

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
