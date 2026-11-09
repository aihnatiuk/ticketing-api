using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Persistence.Configurations;

internal sealed class SectionConfiguration : IEntityTypeConfiguration<Section>
{
    public void Configure(EntityTypeBuilder<Section> builder)
    {
        builder.ToTable("Sections");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).ValueGeneratedOnAdd();

        builder.Property(s => s.Label).HasMaxLength(100).IsRequired();
        builder.Property(s => s.Type).HasConversion<short>().IsRequired();

        builder
            .HasOne(s => s.Venue)
            .WithMany(v => v.Sections)
            .HasForeignKey(s => s.VenueId)
            .OnDelete(DeleteBehavior.Cascade);

        // Composite FK (PricingZoneId, VenueId) -> PricingZones (Id, VenueId)
        builder
            .HasOne(s => s.PricingZone)
            .WithMany()
            .HasForeignKey(s => new { s.PricingZoneId, s.VenueId })
            .HasPrincipalKey(z => new { z.Id, z.VenueId })
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(s => new { s.VenueId, s.Label }).IsUnique();
    }
}
