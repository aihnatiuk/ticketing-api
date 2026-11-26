using Booking.Domain.Entities;
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

    /// <summary>
    /// Releases the reservation held by an order item that is being removed from the order.
    /// Dedicated seats are only modified in the change tracker; the caller must save changes.
    /// General admission seats are returned to the section immediately.
    /// </summary>
    /// <param name="order">The order the item belongs to.</param>
    /// <param name="item">The order item whose reservation is released.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>
    /// The result indicating whether the release succeeded. A general admission release that could not be
    /// applied is reported as a concurrent modification of the order.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// A dedicated seat reservation is missing or inconsistent with the order item.
    /// </exception>
    Task<Result> ReleaseReservationAsync(
        Order order,
        OrderItem item,
        CancellationToken cancellationToken = default
    );
}
