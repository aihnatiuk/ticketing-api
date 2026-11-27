using BuildingBlocks.Application.Abstractions;
using Payment.Module.Domain;

namespace Payment.Module.Persistence.Repositories;

internal interface IPaymentRepository : IRepository<PaymentTransaction, long>
{
    /// <summary>
    /// Loads the order's pending payment as a tracked entity. An order has at most one.
    /// </summary>
    /// <param name="orderId">Id of an order for which to load the transaction.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Payment transaction if found, null otherwise.</returns>
    Task<PaymentTransaction?> GetPendingByOrderIdAsync(
        long orderId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Checks if there is any completed payment for the given order.
    /// </summary>
    /// <param name="orderId">Id of an order to check.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if has completed payment, false otherwise.</returns>
    Task<bool> AnyCompletedForOrderAsync(
        long orderId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Counts the failed payment attempts of the given order.
    /// </summary>
    /// <param name="orderId">Id of an order to count failed payments for.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Number of payments with the Failed status.</returns>
    Task<int> CountFailedForOrderAsync(long orderId, CancellationToken cancellationToken = default);
}
