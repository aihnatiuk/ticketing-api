using BuildingBlocks.Contracts;
using BuildingBlocks.Domain.Results;
using Payment.Contracts.Responses;

namespace Payment.Contracts;

public interface IPaymentModule : IModule
{
    const string Name = "payment";

    static string IModule.ModuleName => Name;

    /// <summary>
    /// Registers a pending payment for the order. The call is idempotent per order:
    /// while the existing payment is still pending, it is returned (with the amount refreshed)
    /// instead of creating a second one.
    /// </summary>
    Task<Result<PaymentResponse>> InitiatePaymentAsync(
        long orderId,
        string currency,
        decimal amount,
        CancellationToken cancellationToken = default
    );
}
