using Booking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Booking.Infrastructure.Persistence.Configurations;

internal sealed class DedicatedSeatConfiguration : IEntityTypeConfiguration<DedicatedSeat>
{
    public void Configure(EntityTypeBuilder<DedicatedSeat> builder)
    {
        builder.ToTable("DedicatedSeats");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).ValueGeneratedOnAdd();

        // Status is the optimistic concurrency token: two buyers racing for the same seat
        // both expect Available, only one UPDATE ... WHERE Status = Available succeeds.
        builder.Property(s => s.Status).HasConversion<short>().IsConcurrencyToken().IsRequired();

        builder.HasIndex(s => new { s.EventId, s.SeatId }).IsUnique();

        builder
            .HasOne<Order>()
            .WithMany()
            .HasForeignKey(s => s.OrderId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
