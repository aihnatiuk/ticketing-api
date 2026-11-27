using BuildingBlocks.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Payment.Module.Domain;
using Payment.Module.Domain.Enums;

namespace Payment.Module.Persistence.Repositories;

internal sealed class PaymentRepository(PaymentDbContext context)
    : BaseRepository<PaymentTransaction, long>(context),
        IPaymentRepository
{
    public Task<PaymentTransaction?> GetPendingByOrderIdAsync(
        long orderId,
        CancellationToken cancellationToken = default
    ) =>
        Entities.FirstOrDefaultAsync(
            p => p.OrderId == orderId && p.Status == PaymentStatus.Pending,
            cancellationToken
        );

    public Task<bool> AnyCompletedForOrderAsync(
        long orderId,
        CancellationToken cancellationToken = default
    ) =>
        Entities.AnyAsync(
            p => p.OrderId == orderId && p.Status == PaymentStatus.Completed,
            cancellationToken
        );

    public Task<int> CountFailedForOrderAsync(
        long orderId,
        CancellationToken cancellationToken = default
    ) =>
        Entities.CountAsync(
            p => p.OrderId == orderId && p.Status == PaymentStatus.Failed,
            cancellationToken
        );
}
