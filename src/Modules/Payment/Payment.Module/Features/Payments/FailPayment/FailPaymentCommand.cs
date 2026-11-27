using BuildingBlocks.Application.Abstractions;

namespace Payment.Module.Features.Payments.FailPayment;

public sealed record FailPaymentCommand(long PaymentId) : ICommand;
