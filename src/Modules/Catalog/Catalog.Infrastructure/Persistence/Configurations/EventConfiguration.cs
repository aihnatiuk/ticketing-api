using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Persistence.Configurations;

internal sealed class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.ToTable(
            "Events",
            t => t.HasCheckConstraint("CK_Events_EndsAt_After_StartsAt", "[EndsAt] > [StartsAt]")
        );

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();

        builder.Property(e => e.Title).HasMaxLength(200).IsRequired();
        builder.Property(e => e.Description).HasMaxLength(2000);

        builder.Property(e => e.StartsAt).IsRequired();
        builder.Property(e => e.EndsAt).IsRequired();

        builder
            .Property(e => e.Currency)
            .HasMaxLength(3)
            .IsFixedLength()
            .IsUnicode(false)
            .IsRequired(); // ISO 4217

        builder.Property(e => e.Status).HasConversion<short>().IsRequired();

        builder
            .HasOne(e => e.Venue)
            .WithMany()
            .HasForeignKey(e => e.VenueId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
