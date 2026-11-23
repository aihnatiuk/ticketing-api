using System.Text.Json;
using BuildingBlocks.Contracts;
using BuildingBlocks.Infrastructure.Messaging.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace BuildingBlocks.Infrastructure.Messaging.RabbitMq;

/// <summary>
/// Consumes every subscription of one module through a channel that only this module uses,
/// and dispatches the messages to the module's integration event handlers.
/// </summary>
internal sealed class RabbitMqConsumerHostedService<TModule>(
    IEnumerable<IntegrationEventSubscription<TModule>> subscriptions,
    RabbitMqConnectionProvider connectionProvider,
    IServiceScopeFactory scopeFactory,
    ILogger<RabbitMqConsumerHostedService<TModule>> logger
) : BackgroundService
    where TModule : IModule
{
    private const ushort PrefetchCount = 10;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        IntegrationEventSubscription<TModule>[] all = [.. subscriptions];
        if (all.Length == 0)
        {
            return;
        }

        // Validate the configuration on startup to fail fast.
        await using (AsyncServiceScope scope = scopeFactory.CreateAsyncScope())
        {
            foreach (IntegrationEventSubscription<TModule> subscription in all)
            {
                subscription.Validate(scope.ServiceProvider);
            }
        }

        try
        {
            await using IChannel channel = await StartAsync(all, stoppingToken);
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            // Host is shutting down.
        }
    }

    private async Task<IChannel> StartAsync(
        IntegrationEventSubscription<TModule>[] all,
        CancellationToken stoppingToken
    )
    {
        IChannel? channel = null;

        try
        {
            IConnection connection = await connectionProvider.GetConsumerConnectionAsync(
                stoppingToken
            );
            channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);

            await RabbitMqTopology.DeclareDeadLetterExchangeAsync(
                channel,
                ModuleMessagingSettings<TModule>.DeadLetterExchange,
                stoppingToken
            );

            // Limit the number of unacknowledged messages to avoid overwhelming the consumer.
            await channel.BasicQosAsync(0, PrefetchCount, global: false, stoppingToken);

            foreach (IntegrationEventSubscription<TModule> subscription in all)
            {
                await RabbitMqTopology.DeclareEventsExchangeAsync(
                    channel,
                    subscription.SourceExchange,
                    stoppingToken
                );
                await RabbitMqTopology.DeclareConsumerQueueAsync(
                    channel,
                    subscription.QueueName,
                    subscription.SourceExchange,
                    subscription.RoutingKey,
                    ModuleMessagingSettings<TModule>.DeadLetterExchange,
                    stoppingToken
                );

                var consumer = new AsyncEventingBasicConsumer(channel);
                consumer.ReceivedAsync += (_, args) =>
                    OnReceivedAsync(channel, subscription, args, stoppingToken);

                await channel.BasicConsumeAsync(
                    subscription.QueueName,
                    autoAck: false,
                    consumer,
                    stoppingToken
                );
            }

            return channel;
        }
        catch (Exception exception)
        {
            if (channel is not null)
            {
                await channel.DisposeAsync();
            }

            if (exception is OperationCanceledException && stoppingToken.IsCancellationRequested)
            {
                throw;
            }

            throw new InvalidOperationException(
                $"Failed to start RabbitMQ consumer for module {TModule.ModuleName}.",
                exception
            );
        }
    }

    private async Task OnReceivedAsync(
        IChannel channel,
        IntegrationEventSubscription<TModule> subscription,
        BasicDeliverEventArgs args,
        CancellationToken cancellationToken
    )
    {
        try
        {
            if (!Guid.TryParse(args.BasicProperties.MessageId, out Guid messageId))
            {
                throw new JsonException(
                    $"Message has no valid {nameof(args.BasicProperties.MessageId)}."
                );
            }

            await using AsyncServiceScope scope = scopeFactory.CreateAsyncScope();
            await subscription.ProcessAsync(
                scope.ServiceProvider,
                messageId,
                args.Body,
                cancellationToken
            );

            await channel.BasicAckAsync(args.DeliveryTag, multiple: false, cancellationToken);
        }
        catch (Exception exception) when (!cancellationToken.IsCancellationRequested)
        {
            logger.LogError(
                exception,
                "Handling message {MessageId} on {Queue} failed. Dead-lettering the message.",
                args.BasicProperties.MessageId,
                subscription.QueueName
            );
            await channel.BasicNackAsync(
                args.DeliveryTag,
                multiple: false,
                requeue: false,
                cancellationToken
            );
        }
    }
}
