using Booking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Booking.Infrastructure.Persistence.Configurations;

internal sealed class GeneralSeatConfiguration : IEntityTypeConfiguration<GeneralSeat>
{
    public void Configure(EntityTypeBuilder<GeneralSeat> builder)
    {
        builder.ToTable(
            "GeneralSeats",
            t =>
                t.HasCheckConstraint(
                    "CK_GeneralSeats_Availability",
                    "[AvailableSeats] >= 0 AND [AvailableSeats] <= [TotalSeats]"
                )
        );

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).ValueGeneratedOnAdd();

        builder.Property(s => s.TotalSeats).IsRequired();
        builder.Property(s => s.AvailableSeats).IsRequired();

        builder.HasIndex(s => new { s.EventId, s.SectionId }).IsUnique();
    }
}
