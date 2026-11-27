using Booking.Application.Abstractions;
using Booking.Application.Features.Orders.Common.Services;
using Booking.Domain.Entities;
using Booking.Domain.Enums;
using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Messaging;
using Payment.Contracts.IntegrationEvents;

namespace Booking.Application.EventHandlers;

/// <summary>
/// Returns the order to pending so the payment can be retried, or gives the order up
/// and releases its seats once the allowed number of failed attempts is reached.
/// </summary>
internal sealed class PaymentFailedIntegrationEventHandler(
    IOrderRepository orders,
    IOrderingService orderingService,
    IUnitOfWork<BookingModule> unitOfWork
) : IIntegrationEventHandler<PaymentFailedIntegrationEvent>
{
    public async Task HandleAsync(
        PaymentFailedIntegrationEvent integrationEvent,
        CancellationToken cancellationToken = default
    )
    {
        Order? order = await orders.GetWithItemsAsync(integrationEvent.OrderId, cancellationToken);
        if (order is null)
        {
            throw new InvalidOperationException(
                $"Payment {integrationEvent.PaymentId} failed for order {integrationEvent.OrderId}, which does not exist."
            );
        }

        if (
            order.Status != OrderStatus.PaymentProcessing
            || order.PaymentId != integrationEvent.PaymentId
        )
        {
            return;
        }

        bool attemptsExhausted = order.FailPayment(integrationEvent.FailedAttempts);
        if (attemptsExhausted)
        {
            await orderingService.ReleaseAllReservationsAsync(order, cancellationToken);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
