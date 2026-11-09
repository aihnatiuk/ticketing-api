using Booking.Domain.Entities;
using BuildingBlocks.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Booking.Application.Abstractions;

public interface IBookingDbContext : IUnitOfWork<BookingModule>
{
    DbSet<DedicatedSeat> DedicatedSeats { get; }

    DbSet<GeneralSeat> GeneralSeats { get; }

    DbSet<Order> Orders { get; }

    DbSet<OrderItem> OrderItems { get; }
}
