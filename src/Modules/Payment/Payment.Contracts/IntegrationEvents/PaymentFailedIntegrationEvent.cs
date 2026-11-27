using BuildingBlocks.Contracts;

namespace Payment.Contracts.IntegrationEvents;

/// <summary>
/// A payment attempt failed.
/// </summary>
/// <param name="PaymentId">The failed payment.</param>
/// <param name="OrderId">The order the payment belongs to.</param>
/// <param name="FailedAttempts">Number of failed payments of the order, including this one.</param>
[IntegrationEvent(IPaymentModule.Name, "payment-failed")]
public sealed record PaymentFailedIntegrationEvent(
    long PaymentId,
    long OrderId,
    int FailedAttempts
);
