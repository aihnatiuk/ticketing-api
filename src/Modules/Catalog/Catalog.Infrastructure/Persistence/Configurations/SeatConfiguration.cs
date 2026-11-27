using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Persistence.Configurations;

internal sealed class SeatConfiguration : IEntityTypeConfiguration<Seat>
{
    public void Configure(EntityTypeBuilder<Seat> builder)
    {
        builder.ToTable("Seats");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).ValueGeneratedOnAdd();

        builder.Property(s => s.Label).HasMaxLength(50).IsRequired();

        builder
            .HasOne(s => s.Row)
            .WithMany(r => r.Seats)
            .HasForeignKey(s => s.RowId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(s => new { s.RowId, s.Label }).IsUnique();
    }
}
