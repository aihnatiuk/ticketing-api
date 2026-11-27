using Booking.Application.Abstractions;
using Booking.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Booking.Infrastructure.Persistence;

internal sealed class BookingDbContext(DbContextOptions<BookingDbContext> options)
    : DbContext(options),
        IBookingDbContext
{
    public const string Schema = "booking";

    public DbSet<DedicatedSeat> DedicatedSeats => Set<DedicatedSeat>();

    public DbSet<GeneralSeat> GeneralSeats => Set<GeneralSeat>();

    public DbSet<Order> Orders => Set<Order>();

    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BookingDbContext).Assembly);
    }
}
