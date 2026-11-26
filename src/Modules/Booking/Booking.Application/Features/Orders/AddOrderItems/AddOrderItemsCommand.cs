using Booking.Application.Features.Orders.Common;
using BuildingBlocks.Application.Abstractions;

namespace Booking.Application.Features.Orders.AddOrderItems;

public sealed record AddOrderItemsCommand(long OrderId, OrderItemsRequestDto? Items)
    : ICommand<OrderDto>;
