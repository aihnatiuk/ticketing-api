using Booking.Application.Abstractions;
using Booking.Domain.Entities;
using Booking.Domain.Enums;
using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Results;
using Microsoft.EntityFrameworkCore;
using Payment.Contracts;
using Payment.Contracts.Responses;

namespace Booking.Application.Features.Orders.InitiatePayment;

internal sealed class InitiatePaymentCommandHandler(
    IOrderRepository orders,
    IPaymentModule paymentModule,
    IUnitOfWork<BookingModule> unitOfWork
) : ICommandHandler<InitiatePaymentCommand, PaymentInitiatedDto>
{
    public async Task<Result<PaymentInitiatedDto>> HandleAsync(
        InitiatePaymentCommand command,
        CancellationToken cancellationToken = default
    )
    {
        Order? order = await orders.GetWithItemsAsync(command.OrderId, cancellationToken);
        if (order is null)
        {
            return OrderErrors.NotFound(command.OrderId);
        }

        // Retrying an already initiated payment returns the same payment.
        if (order is { Status: OrderStatus.PaymentProcessing, PaymentId: not null })
        {
            return new PaymentInitiatedDto(order.Id, order.PaymentId.Value);
        }

        if (order.Status != OrderStatus.Pending)
        {
            return OrderErrors.NotPayable(order.Id);
        }

        if (order.IsExpired())
        {
            return OrderErrors.Expired(order.Id);
        }

        if (order.OrderItems.Count == 0 || order.TotalPrice <= 0)
        {
            return OrderErrors.EmptyOrder(order.Id);
        }

        Result<PaymentResponse> payment = await paymentModule.InitiatePaymentAsync(
            order.Id,
            order.Currency,
            order.TotalPrice,
            cancellationToken
        );
        if (payment.IsFailure)
        {
            return payment.Error;
        }

        order.StartPayment(payment.Value.PaymentId);

        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            return OrderErrors.ConcurrentModification(order.Id);
        }

        return new PaymentInitiatedDto(order.Id, payment.Value.PaymentId);
    }
}
