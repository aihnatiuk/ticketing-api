using Booking.Application.Abstractions;
using Booking.Application.Features.Orders.Common;
using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Results;
using Microsoft.EntityFrameworkCore;

namespace Booking.Application.Features.Orders.GetOrder;

internal sealed class GetOrderQueryHandler(IBookingDbContext db)
    : IQueryHandler<GetOrderQuery, OrderDto>
{
    public async Task<Result<OrderDto>> HandleAsync(
        GetOrderQuery query,
        CancellationToken cancellationToken = default
    )
    {
        OrderDto? order = await db
            .Orders.AsNoTracking()
            .Where(o => o.Id == query.OrderId)
            .Include(o => o.OrderItems)
            .Select(o => new OrderDto(
                o.Id,
                o.CreatedAt,
                o.ExpiresAt,
                o.UserId,
                o.EventId,
                o.Status,
                o.Currency,
                o.TotalPrice,
                o.PaymentId,
                o.OrderItems.Select(oi => new OrderItemDto(
                        oi.Id,
                        oi.CatalogRefId,
                        oi.Type,
                        oi.TicketTypeId,
                        oi.Quantity,
                        oi.UnitPrice
                    ))
                    .ToList()
            ))
            .FirstOrDefaultAsync(cancellationToken);

        if (order is null)
        {
            return OrderErrors.NotFound(query.OrderId);
        }

        return order;
    }
}
