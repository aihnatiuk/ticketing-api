namespace BuildingBlocks.Infrastructure.Messaging.Outbox;

/// <summary>
/// A message waiting to be delivered to the message broker.
/// </summary>
public sealed class OutboxMessage
{
    public Guid Id { get; init; }

    /// <summary>
    /// Full name of the message contract type.
    /// </summary>
    public required string Type { get; init; }

    public required string Payload { get; init; }

    public DateTimeOffset OccurredAt { get; init; }

    public DateTimeOffset? ProcessedAt { get; set; }

    public int Attempts { get; set; }

    public string? Error { get; set; }
}
