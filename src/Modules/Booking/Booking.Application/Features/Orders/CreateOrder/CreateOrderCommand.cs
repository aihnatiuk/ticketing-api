using Booking.Application.Features.Orders.Common;
using BuildingBlocks.Application.Abstractions;

namespace Booking.Application.Features.Orders.CreateOrder;

public sealed record CreateOrderCommand(long EventId, OrderItemsRequestDto? Items)
    : ICommand<OrderDto>;
