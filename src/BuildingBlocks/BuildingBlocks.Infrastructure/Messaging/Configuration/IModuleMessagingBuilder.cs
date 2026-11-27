using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Contracts;
using BuildingBlocks.Infrastructure.Messaging.Outbox;
using BuildingBlocks.Infrastructure.Messaging.RabbitMq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Infrastructure.Messaging.Configuration;

/// <summary>
/// Configures the module's messaging services and event subscriptions.
/// </summary>
/// <typeparam name="TModule">The module type.</typeparam>
/// <typeparam name="TContext">The module's DbContext, which owns its outbox and inbox tables.</typeparam>
public interface IModuleMessagingBuilder<TModule, TContext>
    where TModule : IModule
    where TContext : DbContext
{
    /// <summary>
    /// Publishes the module's outbox messages to the module's own events exchange.
    /// The DbContext must call <see cref="OutboxModelBuilderExtensions.ApplyOutboxConfiguration(ModelBuilder)"/>
    /// </summary>
    IModuleMessagingBuilder<TModule, TContext> UseOutbox();

    /// <summary>
    /// Consumes <typeparamref name="TEvent"/> published by another module on a queue owned by this module.
    /// The DbContext must call <see cref="InboxModelBuilderExtensions.ApplyInboxConfiguration(ModelBuilder)"/>,
    /// and at least one handler must be registered with <see cref="Application.DependencyInjection.AddIntegrationEventHandlers{TModule}(IServiceCollection)"/>.
    /// </summary>
    IModuleMessagingBuilder<TModule, TContext> Subscribe<TEvent>()
        where TEvent : class;
}

internal sealed class ModuleMessagingBuilder<TModule, TContext>(IServiceCollection services)
    : IModuleMessagingBuilder<TModule, TContext>
    where TModule : IModule
    where TContext : DbContext
{
    public IModuleMessagingBuilder<TModule, TContext> UseOutbox()
    {
        services.AddScoped<IOutbox<TModule>>(sp => new DbContextOutbox<TModule>(
            sp.GetRequiredService<TContext>()
        ));
        services.AddSingleton<RabbitMqPublisher<TModule>>();
        services.AddSingleton<IMessagePublisher<TModule>>(sp =>
            sp.GetRequiredService<RabbitMqPublisher<TModule>>()
        );
        services.AddHostedService<OutboxProcessor<TModule, TContext>>();

        return this;
    }

    public IModuleMessagingBuilder<TModule, TContext> Subscribe<TEvent>()
        where TEvent : class
    {
        services.AddSingleton(
            IntegrationEventSubscription<TModule>.Create<TEvent, TContext>(
                consumerModuleName: TModule.ModuleName
            )
        );
        services.AddHostedService<RabbitMqConsumerHostedService<TModule>>();

        return this;
    }
}
