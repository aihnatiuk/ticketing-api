using Booking.Domain.Entities;
using Booking.Domain.Enums;

namespace Booking.Application.Features.Orders.Common;

public sealed record OrderItemDto(
    long Id,
    long CatalogRefId,
    OrderItemType Type,
    long TicketTypeId,
    int Quantity,
    decimal UnitPrice
)
{
    public static OrderItemDto FromDomain(OrderItem item) =>
        new(
            item.Id,
            item.CatalogRefId,
            item.Type,
            item.TicketTypeId,
            item.Quantity,
            item.UnitPrice
        );
}
