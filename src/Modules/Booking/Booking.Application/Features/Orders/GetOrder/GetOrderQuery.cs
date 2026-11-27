using Booking.Application.Features.Orders.Common;
using BuildingBlocks.Application.Abstractions;

namespace Booking.Application.Features.Orders.GetOrder;

public sealed record GetOrderQuery(long OrderId) : IQuery<OrderDto>;
