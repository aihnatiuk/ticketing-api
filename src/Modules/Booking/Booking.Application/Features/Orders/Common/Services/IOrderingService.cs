using BuildingBlocks.Domain.Results;

namespace Booking.Application.Features.Orders.Common.Services;

public interface IOrderingService
{
    /// <summary>
    /// Checks that the requested items can be reserved, asks Catalog for their prices and builds the order items.
    /// </summary>
    /// <param name="eventId">An Id of an event for which ordering is being performed.</param>
    /// <param name="request">Contains lists of dedicated and general admission seats that are being ordered.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The result indicating whether validation checks passed; order details.</returns>
    Task<Result<PreparedOrderDetails>> PrepareOrderDetailsAsync(
        long eventId,
        OrderItemsRequestDto? request,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Tries to reserve the prepared and validated items for the order.
    /// </summary>
    /// <param name="orderId">The identifier of the order for which to reserve seats.</param>
    /// <param name="orderDetails">Details of the order, required for reservation.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The result indicating whether reservation succeeded.</returns>
    Task<Result> ReserveAsync(
        long orderId,
        PreparedOrderDetails orderDetails,
        CancellationToken cancellationToken = default
    );
}
