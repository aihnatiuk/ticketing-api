using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Persistence.Configurations;

internal sealed class PricingZoneConfiguration : IEntityTypeConfiguration<PricingZone>
{
    public void Configure(EntityTypeBuilder<PricingZone> builder)
    {
        builder.ToTable("PricingZones");

        builder.HasKey(z => z.Id);
        builder.Property(z => z.Id).ValueGeneratedOnAdd();

        builder.Property(z => z.Label).HasMaxLength(100).IsRequired();

        builder
            .HasOne<Venue>()
            .WithMany(v => v.PricingZones)
            .HasForeignKey(z => z.VenueId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(z => new { z.VenueId, z.Label }).IsUnique();

        // Target of the composite FK from Section: guarantees a section can only use a zone of its own venue.
        builder.HasAlternateKey(z => new { z.Id, z.VenueId });
    }
}
