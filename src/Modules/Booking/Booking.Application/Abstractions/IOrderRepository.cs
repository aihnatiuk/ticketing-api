using Booking.Domain.Entities;
using BuildingBlocks.Application.Abstractions;

namespace Booking.Application.Abstractions;

public interface IOrderRepository : IRepository<Order, long>
{
    /// <summary>
    /// Loads the order together with its items.
    /// </summary>
    /// <param name="id">The order id to load.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Tracked Order with order items.</returns>
    Task<Order?> GetWithItemsAsync(long id, CancellationToken cancellationToken = default);
}
