namespace BuildingBlocks.Infrastructure.Messaging.Outbox;

/// <summary>
/// Records that a consumer has already processed a message, so redeliveries can be skipped.
/// </summary>
public sealed class InboxMessage
{
    public Guid MessageId { get; init; }

    /// <summary>
    /// Name of the consuming queue. The same message may be consumed by several consumers.
    /// </summary>
    public required string Consumer { get; init; }

    public DateTimeOffset ProcessedAt { get; init; }
}
