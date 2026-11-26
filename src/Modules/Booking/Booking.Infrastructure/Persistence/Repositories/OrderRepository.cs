using Booking.Application.Abstractions;
using Booking.Domain.Entities;
using BuildingBlocks.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Booking.Infrastructure.Persistence.Repositories;

internal sealed class OrderRepository(BookingDbContext context)
    : BaseRepository<Order, long>(context),
        IOrderRepository
{
    public async Task<Order?> GetWithItemsAsync(
        long id,
        CancellationToken cancellationToken = default
    ) =>
        await Entities
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
}
