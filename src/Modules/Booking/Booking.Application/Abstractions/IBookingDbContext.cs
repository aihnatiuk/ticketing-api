using Booking.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Booking.Application.Abstractions;

public interface IBookingDbContext
{
    DbSet<DedicatedSeat> DedicatedSeats { get; }

    DbSet<GeneralSeat> GeneralSeats { get; }

    DbSet<Order> Orders { get; }

    DbSet<OrderItem> OrderItems { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
