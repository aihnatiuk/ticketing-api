using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Persistence.Configurations;

internal sealed class VenueConfiguration : IEntityTypeConfiguration<Venue>
{
    public void Configure(EntityTypeBuilder<Venue> builder)
    {
        builder.ToTable("Venues");

        builder.HasKey(v => v.Id);
        builder.Property(v => v.Id).ValueGeneratedOnAdd();

        builder.Property(v => v.Name).HasMaxLength(200).IsRequired();

        // Address is a record (value object) stored in the Venues table as Address_* columns.
        builder.OwnsOne(
            v => v.Address,
            a =>
            {
                a.Property(x => x.Street).HasMaxLength(200).IsRequired();
                a.Property(x => x.City).HasMaxLength(100).IsRequired();
                a.Property(x => x.CountryCode)
                    .HasMaxLength(2)
                    .IsFixedLength()
                    .IsUnicode(false)
                    .IsRequired(); // ISO 3166-1 alpha-2
            }
        );
        builder.Navigation(v => v.Address).IsRequired();
    }
}
