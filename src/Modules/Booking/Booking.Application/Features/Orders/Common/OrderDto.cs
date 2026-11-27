using Booking.Domain.Entities;
using Booking.Domain.Enums;

namespace Booking.Application.Features.Orders.Common;

public sealed record OrderDto(
    long Id,
    DateTimeOffset CreatedAt,
    DateTimeOffset ExpiresAt,
    Guid UserId,
    long EventId,
    OrderStatus Status,
    string Currency,
    decimal TotalPrice,
    long? PaymentId,
    IReadOnlyList<OrderItemDto> Items
)
{
    public static OrderDto FromDomain(Order order) =>
        new(
            order.Id,
            order.CreatedAt,
            order.ExpiresAt,
            order.UserId,
            order.EventId,
            order.Status,
            order.Currency,
            order.TotalPrice,
            order.PaymentId,
            order.OrderItems.Select(OrderItemDto.FromDomain).ToList()
        );
}
