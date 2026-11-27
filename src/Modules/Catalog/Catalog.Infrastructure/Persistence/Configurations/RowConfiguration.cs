using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Persistence.Configurations;

internal sealed class RowConfiguration : IEntityTypeConfiguration<Row>
{
    public void Configure(EntityTypeBuilder<Row> builder)
    {
        builder.ToTable("Rows");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedOnAdd();

        builder.Property(r => r.Label).HasMaxLength(50).IsRequired();

        builder
            .HasOne(r => r.Section)
            .WithMany(s => s.Rows)
            .HasForeignKey(r => r.SectionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(r => new { r.SectionId, r.Label }).IsUnique();
    }
}
