using System.Text;
using RabbitMQ.Client;

namespace BuildingBlocks.Infrastructure.Messaging.RabbitMq;

/// <summary>
/// Publishes a module's outbox messages through a channel that only this module uses.
/// </summary>
internal sealed class RabbitMqPublisher<TModule>(
    RabbitMqConnectionProvider connectionProvider,
    ModuleMessagingSettings<TModule> settings
) : IMessagePublisher<TModule>, IAsyncDisposable
{
    private IChannel? _channel;

    public async Task PublishAsync(
        Guid messageId,
        string routingKey,
        string payload,
        CancellationToken cancellationToken = default
    )
    {
        IChannel channel = await GetChannelAsync(cancellationToken);

        var properties = new BasicProperties
        {
            MessageId = messageId.ToString(),
            ContentType = "application/json",
            DeliveryMode = DeliveryModes.Persistent,
            Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds()),
        };

        // mandatory + publisher confirms: makes this throw,
        // so the outbox keeps the row and retries instead of silently losing the message.
        await channel.BasicPublishAsync(
            settings.Exchange,
            routingKey: routingKey,
            mandatory: true,
            basicProperties: properties,
            body: Encoding.UTF8.GetBytes(payload),
            cancellationToken: cancellationToken
        );
    }

    private async Task<IChannel> GetChannelAsync(CancellationToken cancellationToken)
    {
        if (_channel is { IsOpen: true })
        {
            return _channel;
        }

        if (_channel is not null)
        {
            await _channel.DisposeAsync();
            _channel = null;
        }

        IConnection connection = await connectionProvider.GetPublisherConnectionAsync(
            cancellationToken
        );
        IChannel channel = await connection.CreateChannelAsync(
            new CreateChannelOptions(
                publisherConfirmationsEnabled: true,
                publisherConfirmationTrackingEnabled: true
            ),
            cancellationToken
        );
        await RabbitMqTopology.DeclareEventsExchangeAsync(
            channel,
            settings.Exchange,
            cancellationToken
        );

        _channel = channel;

        return channel;
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel is not null)
        {
            await _channel.DisposeAsync();
        }
    }
}
