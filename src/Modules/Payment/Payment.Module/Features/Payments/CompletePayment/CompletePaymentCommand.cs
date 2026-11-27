using BuildingBlocks.Application.Abstractions;

namespace Payment.Module.Features.Payments.CompletePayment;

public sealed record CompletePaymentCommand(long PaymentId) : ICommand;
