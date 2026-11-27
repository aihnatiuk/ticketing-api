using BuildingBlocks.Application.Abstractions;

namespace Booking.Application.Features.Orders.InitiatePayment;

public sealed record InitiatePaymentCommand(long OrderId) : ICommand<PaymentInitiatedDto>;

public sealed record PaymentInitiatedDto(long OrderId, long PaymentId);
