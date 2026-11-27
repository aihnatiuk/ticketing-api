using Booking.Domain.Entities;
using Booking.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Booking.Infrastructure.Persistence.Configurations;

internal sealed class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable(
            "OrderItems",
            t =>
            {
                t.HasCheckConstraint(
                    "CK_OrderItems_Quantity",
                    $"[Quantity] > 0 AND ([Type] <> {(short)OrderItemType.DedicatedSeat} OR [Quantity] = 1)"
                );
                t.HasCheckConstraint("CK_OrderItems_UnitPrice_NonNegative", "[UnitPrice] >= 0");
            }
        );

        builder.HasKey(i => i.Id);
        builder.Property(i => i.Id).ValueGeneratedOnAdd();

        builder.Property(i => i.Type).HasConversion<short>().IsRequired();
        builder.Property(i => i.UnitPrice).HasPrecision(18, 2).IsRequired();

        builder
            .HasOne(i => i.Order)
            .WithMany(o => o.OrderItems)
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
