using Booking.Application.Abstractions;
using Booking.Application.Features.Orders.Common;
using Booking.Application.Features.Orders.Common.Services;
using Booking.Domain.Entities;
using Booking.Domain.Enums;
using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Booking.Application.Features.Orders.AddOrderItems;

internal sealed class AddOrderItemsCommandHandler(
    IOrderRepository orders,
    IOrderingService orderingService,
    IUnitOfWork<BookingModule> unitOfWork
) : ICommandHandler<AddOrderItemsCommand, OrderDto>
{
    public async Task<Result<OrderDto>> HandleAsync(
        AddOrderItemsCommand command,
        CancellationToken cancellationToken = default
    )
    {
        if (command.Items is null || command.Items is { DedicatedSeats: [], GeneralAdmissions: [] })
        {
            return OrderErrors.NoItemsRequested();
        }

        Order? order = await orders.GetWithItemsAsync(command.OrderId, cancellationToken);
        if (order is null)
        {
            return OrderErrors.NotFound(command.OrderId);
        }

        if (order.Status != OrderStatus.Pending)
        {
            return OrderErrors.NotPending(order.Id);
        }

        if (order.IsExpired())
        {
            return OrderErrors.Expired(order.Id);
        }

        Result<PreparedOrderDetails> detailsResult = await orderingService.PrepareOrderDetailsAsync(
            order.EventId,
            command.Items,
            cancellationToken
        );
        if (detailsResult.IsFailure)
        {
            return detailsResult.Error;
        }

        PreparedOrderDetails orderDetails = detailsResult.Value;
        if (orderDetails.Currency != order.Currency)
        {
            throw new InvalidOperationException(
                $"Currency of event '{order.EventId}' is '{orderDetails.Currency}', but order '{order.Id}' was created in '{order.Currency}'."
            );
        }

        await using ITransaction transaction = await unitOfWork.BeginTransactionAsync(
            cancellationToken
        );

        order.AddItems(orderDetails.OrderItems);

        try
        {
            // Separate order revision update concurrency conflicts from seat reservation concurrency conflicts.
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            return OrderErrors.ConcurrentModification(order.Id);
        }

        Result reserveResult = await orderingService.ReserveAsync(
            order.Id,
            orderDetails,
            cancellationToken
        );
        if (reserveResult.IsFailure)
        {
            return reserveResult.Error;
        }

        await transaction.CommitAsync(cancellationToken);

        return OrderDto.FromDomain(order);
    }
}
