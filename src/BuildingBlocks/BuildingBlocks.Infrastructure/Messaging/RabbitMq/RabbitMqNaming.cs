namespace BuildingBlocks.Infrastructure.Messaging.RabbitMq;

/// <summary>
/// The single source of truth for broker object names.
/// </summary>
internal static class RabbitMqNaming
{
    /// <summary>
    /// The topic exchange owned by the module that publishes events.
    /// </summary>
    public static string EventsExchange(string module) => $"{module}.events";

    /// <summary>
    /// The dead-letter exchange owned by the module that consumes events.
    /// </summary>
    public static string DeadLetterExchange(string module) => $"{module}.dlx";

    /// <summary>
    /// The queue a consuming module uses for one event type.
    /// </summary>
    public static string Queue(string consumerModule, string eventName) =>
        $"{consumerModule}.{eventName}";

    /// <summary>
    /// The dead-letter queue a consuming module uses for one event type.
    /// </summary>
    public static string DeadLetterQueue(string queue) => $"{queue}.dlq";
}
