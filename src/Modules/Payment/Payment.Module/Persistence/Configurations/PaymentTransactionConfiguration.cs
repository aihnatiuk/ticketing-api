using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Payment.Module.Domain;

namespace Payment.Module.Persistence.Configurations;

internal sealed class PaymentTransactionConfiguration : IEntityTypeConfiguration<PaymentTransaction>
{
    public void Configure(EntityTypeBuilder<PaymentTransaction> builder)
    {
        builder.ToTable(
            "PaymentTransactions",
            t => t.HasCheckConstraint("CK_PaymentTransactions_Amount_NonNegative", "[Amount] >= 0")
        );

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedOnAdd();

        builder.Property(p => p.Status).HasConversion<short>().IsConcurrencyToken().IsRequired();

        builder
            .Property(p => p.Currency)
            .HasMaxLength(3)
            .IsFixedLength()
            .IsUnicode(false)
            .IsRequired(); // ISO 4217

        builder.Property(p => p.Amount).HasPrecision(18, 2).IsRequired();

        builder.Property(p => p.CreatedAt).IsRequired();
    }
}
