using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Domain.Results;
using Microsoft.EntityFrameworkCore;
using Payment.Contracts.IntegrationEvents;
using Payment.Module.Domain;
using Payment.Module.Domain.Enums;
using Payment.Module.Persistence.Repositories;

namespace Payment.Module.Features.Payments.CompletePayment;

internal sealed class CompletePaymentCommandHandler(
    IPaymentRepository payments,
    IOutbox<PaymentModule> outbox,
    IUnitOfWork<PaymentModule> unitOfWork
) : ICommandHandler<CompletePaymentCommand>
{
    public async Task<Result> HandleAsync(
        CompletePaymentCommand command,
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

        if (payment.Status == PaymentStatus.Completed)
        {
            return Result.Success();
        }

        if (payment.Status != PaymentStatus.Pending)
        {
            return PaymentErrors.NotCompletable(payment.Id, payment.Status);
        }

        payment.Complete();
        outbox.Add(
            new PaymentSucceededIntegrationEvent(
                payment.Id,
                payment.OrderId,
                payment.Amount,
                payment.Currency
            )
        );

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
