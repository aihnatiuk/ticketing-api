using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Persistence.Configurations;

internal sealed class EventPriceConfiguration : IEntityTypeConfiguration<EventPrice>
{
    public void Configure(EntityTypeBuilder<EventPrice> builder)
    {
        builder.ToTable(
            "EventPrices",
            t => t.HasCheckConstraint("CK_EventPrices_Amount_NonNegative", "[Amount] >= 0")
        );

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedOnAdd();

        builder.Property(p => p.Amount).HasPrecision(18, 2).IsRequired();

        builder
            .HasOne<Event>()
            .WithMany(e => e.Prices)
            .HasForeignKey(p => p.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne<PricingZone>()
            .WithMany()
            .HasForeignKey(p => p.PricingZoneId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(p => p.TicketType)
            .WithMany()
            .HasForeignKey(p => p.TicketTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasIndex(p => new
            {
                p.EventId,
                p.PricingZoneId,
                p.TicketTypeId,
            })
            .IsUnique();
    }
}
