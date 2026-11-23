using System.Text.Json;
using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Contracts;
using BuildingBlocks.Infrastructure.Messaging.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Infrastructure.Messaging.RabbitMq;

/// <summary>
/// Describes one queue consumed by a module and how to process the messages arriving on it.
/// </summary>
/// <param name="QueueName">Name of the queue. Also identifies the consumer in the inbox.</param>
/// <param name="SourceExchange">Events exchange of the publishing module the queue is bound to.</param>
/// <param name="RoutingKey">A routing key for the queue.</param>
/// <param name="Validate">Throws when the module is not set up to process the subscription.</param>
/// <param name="ProcessAsync">Runs the module's handlers for a message with inbox deduplication, atomically.</param>
internal sealed record IntegrationEventSubscription<TModule>(
    string QueueName,
    string SourceExchange,
    string RoutingKey,
    Action<IServiceProvider> Validate,
    Func<IServiceProvider, Guid, ReadOnlyMemory<byte>, CancellationToken, Task> ProcessAsync
)
    where TModule : IModule
{
    public static IntegrationEventSubscription<TModule> Create<TEvent, TContext>(
        string consumerModuleName
    )
        where TEvent : class
        where TContext : DbContext
    {
        IntegrationEventDescriptor descriptor = IntegrationEventDescriptor.From(typeof(TEvent));
        string queueName = RabbitMqNaming.Queue(consumerModuleName, descriptor.EventName);

        return new IntegrationEventSubscription<TModule>(
            queueName,
            SourceExchange: RabbitMqNaming.EventsExchange(descriptor.PublishingModuleName),
            RoutingKey: typeof(TEvent).FullName!,
            Validate: services => EnsureConfigured<TEvent, TContext>(services, queueName),
            ProcessAsync: (services, messageId, body, cancellationToken) =>
                ProcessMessageAsync<TEvent, TContext>(
                    services,
                    queueName,
                    messageId,
                    body,
                    cancellationToken
                )
        );
    }

    private static void EnsureConfigured<TEvent, TContext>(
        IServiceProvider services,
        string queueName
    )
        where TEvent : class
        where TContext : DbContext
    {
        TContext context = services.GetRequiredService<TContext>();
        if (context.Model.FindEntityType(typeof(InboxMessage)) is null)
        {
            throw new InvalidOperationException(
                $"{typeof(TContext).Name} must map {nameof(InboxMessage)} (call ApplyInboxConfiguration) "
                    + $"to consume queue '{queueName}'."
            );
        }

        if (!services.GetKeyedServices<IIntegrationEventHandler<TEvent>>(typeof(TModule)).Any())
        {
            throw new InvalidOperationException(
                $"Queue '{queueName}' has no {nameof(IIntegrationEventHandler<>)} for {typeof(TEvent).Name} "
                    + $"registered for module '{typeof(TModule).Name}' (call AddIntegrationEventHandlers)."
            );
        }
    }

    private static async Task ProcessMessageAsync<TEvent, TContext>(
        IServiceProvider services,
        string queueName,
        Guid messageId,
        ReadOnlyMemory<byte> body,
        CancellationToken cancellationToken
    )
        where TEvent : class
        where TContext : DbContext
    {
        TContext context = services.GetRequiredService<TContext>();

        bool alreadyProcessed = await context
            .Set<InboxMessage>()
            .AnyAsync(m => m.MessageId == messageId && m.Consumer == queueName, cancellationToken);
        if (alreadyProcessed)
        {
            return;
        }

        TEvent? integrationEvent = JsonSerializer.Deserialize<TEvent>(body.Span);
        if (integrationEvent is null)
        {
            throw new JsonException(
                $"Message {messageId} deserialized to null for {typeof(TEvent).Name}."
            );
        }

        // One transaction for all handlers and the inbox row.
        await using IDbContextTransaction transaction =
            await context.Database.BeginTransactionAsync(cancellationToken);

        foreach (
            IIntegrationEventHandler<TEvent> handler in services.GetKeyedServices<
                IIntegrationEventHandler<TEvent>
            >(typeof(TModule))
        )
        {
            await handler.HandleAsync(integrationEvent, cancellationToken);
        }

        context
            .Set<InboxMessage>()
            .Add(
                new InboxMessage
                {
                    MessageId = messageId,
                    Consumer = queueName,
                    ProcessedAt = DateTimeOffset.UtcNow,
                }
            );
        await context.SaveChangesAsync(cancellationToken);

        await transaction.CommitAsync(cancellationToken);
    }
}
