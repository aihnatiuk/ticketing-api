using BuildingBlocks.Domain.Results;

namespace Booking.Application.Features.Orders;

public static class OrderErrors
{
    public static Error NotFound(long orderId) =>
        Error.NotFound("Order.NotFound", $"Order with id: '{orderId}' was not found.");
}
