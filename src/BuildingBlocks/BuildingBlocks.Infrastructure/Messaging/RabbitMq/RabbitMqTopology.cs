using RabbitMQ.Client;

namespace BuildingBlocks.Infrastructure.Messaging.RabbitMq;

/// <summary>
/// Idempotent declarations of broker objects. Every declaration lives here so that the publisher and
/// the consumers, which both declare the same exchange, always use identical arguments.
/// </summary>
internal static class RabbitMqTopology
{
    public static Task DeclareEventsExchangeAsync(
        IChannel channel,
        string exchange,
        CancellationToken cancellationToken
    ) =>
        channel.ExchangeDeclareAsync(
            exchange,
            ExchangeType.Topic,
            durable: true,
            cancellationToken: cancellationToken
        );

    public static Task DeclareDeadLetterExchangeAsync(
        IChannel channel,
        string deadLetterExchange,
        CancellationToken cancellationToken
    ) =>
        channel.ExchangeDeclareAsync(
            deadLetterExchange,
            ExchangeType.Direct,
            durable: true,
            cancellationToken: cancellationToken
        );

    /// <summary>
    /// Declares a queue bound to the source module's events exchange, plus its dead-letter queue.
    /// </summary>
    public static async Task DeclareConsumerQueueAsync(
        IChannel channel,
        string queueName,
        string sourceExchange,
        string routingKey,
        string deadLetterExchange,
        CancellationToken cancellationToken
    )
    {
        string deadLetterQueue = RabbitMqNaming.DeadLetterQueue(queueName);

        await channel.QueueDeclareAsync(
            deadLetterQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: cancellationToken
        );
        await channel.QueueBindAsync(
            deadLetterQueue,
            deadLetterExchange,
            routingKey: queueName,
            cancellationToken: cancellationToken
        );

        await channel.QueueDeclareAsync(
            queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: new Dictionary<string, object?>
            {
                ["x-dead-letter-exchange"] = deadLetterExchange,
                ["x-dead-letter-routing-key"] = queueName,
            },
            cancellationToken: cancellationToken
        );
        await channel.QueueBindAsync(
            queueName,
            sourceExchange,
            routingKey,
            cancellationToken: cancellationToken
        );
    }
}
