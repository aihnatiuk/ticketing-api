using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.Infrastructure.Messaging.Outbox;

public static class OutboxModelBuilderExtensions
{
    /// <summary>
    /// Adds <see cref="OutboxMessage"/> entity to the module's default schema.
    /// </summary>
    public static ModelBuilder ApplyOutboxConfiguration(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OutboxMessage>(builder =>
        {
            builder.ToTable("OutboxMessages");
            builder.HasKey(m => m.Id);
            builder.Property(m => m.Id).ValueGeneratedNever();
            builder.Property(m => m.Type).HasMaxLength(500).IsRequired();
            builder.Property(m => m.Payload).IsRequired();
            builder.Property(m => m.Error).HasMaxLength(2000);
            builder
                .HasIndex(m => m.OccurredAt)
                .HasFilter($"[{nameof(OutboxMessage.ProcessedAt)}] IS NULL")
                .HasDatabaseName("IX_OutboxMessages_Unprocessed");
        });

        return modelBuilder;
    }

    /// <summary>
    /// Adds <see cref="InboxMessage"/> entity to the module's default schema.
    /// </summary>
    public static ModelBuilder ApplyInboxConfiguration(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<InboxMessage>(builder =>
        {
            builder.ToTable("InboxMessages");
            builder.HasKey(m => new { m.MessageId, m.Consumer });
            builder.Property(m => m.Consumer).HasMaxLength(200).IsRequired();
        });

        return modelBuilder;
    }
}
