using BuildingBlocks.Contracts;

namespace Payment.Contracts.IntegrationEvents;

[IntegrationEvent(IPaymentModule.Name, "payment-succeeded")]
public sealed record PaymentSucceededIntegrationEvent(
    long PaymentId,
    long OrderId,
    decimal Amount,
    string Currency
);
