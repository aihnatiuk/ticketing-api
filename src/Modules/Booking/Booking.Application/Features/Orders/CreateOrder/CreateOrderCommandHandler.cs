using Booking.Application.Abstractions;
using Booking.Application.Features.Orders.Common;
using Booking.Application.Features.Orders.Common.Services;
using Booking.Domain.Entities;
using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Results;

namespace Booking.Application.Features.Orders.CreateOrder;

internal sealed class CreateOrderCommandHandler(
    IOrderRepository orders,
    IOrderingService orderingService,
    IUnitOfWork<BookingModule> unitOfWork
) : ICommandHandler<CreateOrderCommand, OrderDto>
{
    public async Task<Result<OrderDto>> HandleAsync(
        CreateOrderCommand command,
        CancellationToken cancellationToken = default
    )
    {
        Result<PreparedOrderDetails> detailsResult = await orderingService.PrepareOrderDetailsAsync(
            command.EventId,
            command.Items,
            cancellationToken
        );
        if (detailsResult.IsFailure)
        {
            return detailsResult.Error;
        }

        PreparedOrderDetails orderDetails = detailsResult.Value;

        Order order = Order.Create(
            userId: Guid.Empty, // TODO: Get a real user Id when authentication is implemented
            command.EventId,
            orderDetails.Currency,
            orderDetails.OrderItems
        );

        await using ITransaction transaction = await unitOfWork.BeginTransactionAsync(
            cancellationToken
        );

        orders.Add(order);
        await unitOfWork.SaveChangesAsync(cancellationToken);

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
