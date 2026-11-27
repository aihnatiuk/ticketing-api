using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Results;
using Microsoft.EntityFrameworkCore;
using Payment.Contracts;
using Payment.Contracts.Responses;
using Payment.Module.Domain;
using Payment.Module.Domain.Enums;
using Payment.Module.Features.Payments;
using Payment.Module.Persistence.Repositories;

namespace Payment.Module;

/// <summary>
/// Represents the Payment module in the application.
/// </summary>
internal sealed class PaymentModule(
    IPaymentRepository payments,
    IUnitOfWork<PaymentModule> unitOfWork
) : IPaymentModule
{
    public async Task<Result<PaymentResponse>> InitiatePaymentAsync(
        long orderId,
        string currency,
        decimal amount,
        CancellationToken cancellationToken = default
    )
    {
        if (amount <= 0)
        {
            return PaymentErrors.InvalidAmount(amount);
        }

        // Never charge an order twice.
        if (await payments.AnyCompletedForOrderAsync(orderId, cancellationToken))
        {
            return PaymentErrors.AlreadyProcessed(orderId, PaymentStatus.Completed);
        }

        PaymentTransaction? payment = await payments.GetPendingByOrderIdAsync(
            orderId,
            cancellationToken
        );
        if (payment is null)
        {
            payment = PaymentTransaction.Create(orderId, currency, amount);
            payments.Add(payment);

            try
            {
                await unitOfWork.SaveChangesAsync(cancellationToken);
                return new PaymentResponse(payment.Id);
            }
            catch (DbUpdateException)
            {
                // A concurrent request created the pending payment first (filtered unique index on OrderId).
                payments.Remove(payment);
                payment = await payments.GetPendingByOrderIdAsync(orderId, cancellationToken);
                if (payment is null)
                {
                    throw;
                }
            }
        }

        if (payment.Amount != amount || payment.Currency != currency)
        {
            payment.UpdateAmount(currency, amount);

            try
            {
                await unitOfWork.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                return PaymentErrors.ConcurrentModification(payment.Id);
            }
        }

        return new PaymentResponse(payment.Id);
    }
}
