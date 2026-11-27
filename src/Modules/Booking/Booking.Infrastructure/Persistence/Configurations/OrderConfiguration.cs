using Booking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Booking.Infrastructure.Persistence.Configurations;

internal sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable(
            "Orders",
            t => t.HasCheckConstraint("CK_Orders_TotalPrice_NonNegative", "[TotalPrice] >= 0")
        );

        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).ValueGeneratedOnAdd();

        builder.Property(o => o.Status).HasConversion<short>().IsRequired();

        builder
            .Property(o => o.Currency)
            .HasMaxLength(3)
            .IsFixedLength()
            .IsUnicode(false)
            .IsRequired(); // ISO 4217

        builder.Property(o => o.TotalPrice).HasPrecision(18, 2).IsRequired();
    }
}
