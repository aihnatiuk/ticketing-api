using Booking.Application.Features.Orders.Common;
using BuildingBlocks.Application.Abstractions;

namespace Booking.Application.Features.Orders.RemoveOrderItem;

public sealed record RemoveOrderItemCommand(long OrderId, long OrderItemId) : ICommand<OrderDto>;
