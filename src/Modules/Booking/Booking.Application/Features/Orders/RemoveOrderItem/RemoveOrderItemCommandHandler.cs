using Booking.Application.Abstractions;
using Booking.Application.Features.Orders.Common;
using Booking.Application.Features.Orders.Common.Services;
using Booking.Domain.Entities;
using Booking.Domain.Enums;
using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Booking.Application.Features.Orders.RemoveOrderItem;

internal sealed class RemoveOrderItemCommandHandler(
    IOrderRepository orders,
    IOrderingService orderingService,
    IUnitOfWork<BookingModule> unitOfWork
) : ICommandHandler<RemoveOrderItemCommand, OrderDto>
{
    public async Task<Result<OrderDto>> HandleAsync(
        RemoveOrderItemCommand command,
        CancellationToken cancellationToken = default
    )
    {
        Order? order = await orders.GetWithItemsAsync(command.OrderId, cancellationToken);
        if (order is null)
        {
            return OrderErrors.NotFound(command.OrderId);
        }

        // Removal is idempotent: an item that is already gone is not an error.
        OrderItem? item = order.OrderItems.FirstOrDefault(i => i.Id == command.OrderItemId);
        if (item is null)
        {
            return OrderDto.FromDomain(order);
        }

        if (order.Status != OrderStatus.Pending)
        {
            return OrderErrors.NotPending(order.Id);
        }

        if (order.IsExpired())
        {
            return OrderErrors.Expired(order.Id);
        }

        await using ITransaction transaction = await unitOfWork.BeginTransactionAsync(
            cancellationToken
        );

        order.RemoveItem(item.Id);

        Result releaseResult = await orderingService.ReleaseReservationAsync(
            order,
            item,
            cancellationToken
        );
        if (releaseResult.IsFailure)
        {
            return releaseResult.Error;
        }

        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            return OrderErrors.ConcurrentModification(order.Id);
        }

        await transaction.CommitAsync(cancellationToken);

        return OrderDto.FromDomain(order);
    }
}
