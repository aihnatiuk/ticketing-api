using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Domain.Results;
using Microsoft.EntityFrameworkCore;
using Payment.Contracts.IntegrationEvents;
using Payment.Module.Domain;
using Payment.Module.Domain.Enums;
using Payment.Module.Persistence.Repositories;

namespace Payment.Module.Features.Payments.FailPayment;

internal sealed class FailPaymentCommandHandler(
    IPaymentRepository payments,
    IOutbox<PaymentModule> outbox,
    IUnitOfWork<PaymentModule> unitOfWork
) : ICommandHandler<FailPaymentCommand>
{
    public async Task<Result> HandleAsync(
        FailPaymentCommand command,
        CancellationToken cancellationToken = default
    )
    {
        PaymentTransaction? payment = await payments.GetByIdAsync(
            command.PaymentId,
            cancellationToken
        );
        if (payment is null)
        {
            return PaymentErrors.NotFound(command.PaymentId);
        }

        if (payment.Status == PaymentStatus.Failed)
        {
            return Result.Success();
        }

        if (payment.Status != PaymentStatus.Pending)
        {
            return PaymentErrors.CannotFail(payment.Id, payment.Status);
        }

        // The current payment is not saved as failed yet, so it is added to the count explicitly.
        int failedAttempts =
            await payments.CountFailedForOrderAsync(payment.OrderId, cancellationToken) + 1;

        payment.Fail();
        outbox.Add(new PaymentFailedIntegrationEvent(payment.Id, payment.OrderId, failedAttempts));

        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            return PaymentErrors.ConcurrentModification(payment.Id);
        }

        return Result.Success();
    }
}
