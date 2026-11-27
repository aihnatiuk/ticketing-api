using BuildingBlocks.Domain.Results;
using Payment.Module.Domain.Enums;

namespace Payment.Module.Features.Payments;

internal static class PaymentErrors
{
    public static Error NotFound(long paymentId) =>
        Error.NotFound("Payment.NotFound", $"Payment {paymentId} was not found.");

    public static Error AlreadyProcessed(long orderId, PaymentStatus status) =>
        Error.Conflict(
            "Payment.AlreadyProcessed",
            $"The payment for order {orderId} is already {status}."
        );

    public static Error NotCompletable(long paymentId, PaymentStatus status) =>
        Error.Conflict(
            "Payment.NotCompletable",
            $"Payment {paymentId} is {status} and cannot be completed."
        );

    public static Error InvalidAmount(decimal amount) =>
        Error.Validation(
            "Payment.InvalidAmount",
            $"The payment amount must be greater than zero, but was {amount}."
        );

    public static Error ConcurrentModification(long paymentId) =>
        Error.Conflict(
            "Payment.ConcurrentModification",
            $"Payment {paymentId} was modified concurrently. Retry the request."
        );
}
