using BuildingBlocks.Contracts;
using BuildingBlocks.Infrastructure.Messaging.RabbitMq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BuildingBlocks.Infrastructure.Messaging.Configuration;

public static class MessagingServiceCollectionExtensions
{
    /// <summary>
    /// Registers common RabbitMQ services that all modules share. Idempotent.
    /// </summary>
    public static IServiceCollection AddRabbitMq(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        if (services.Any(d => d.ServiceType == typeof(RabbitMqConnectionProvider)))
        {
            return services;
        }

        services
            .AddOptions<RabbitMqOptions>()
            .Bind(configuration.GetSection(RabbitMqOptions.SectionName));
        services.TryAddSingleton<RabbitMqConnectionProvider>();

        return services;
    }

    /// <summary>
    /// Registers the module's own messaging services (publisher, consumers, outbox processor) on top of
    /// the shared RabbitMQ setup. Each module gets its own exchange, queues and channels.
    /// </summary>
    /// <typeparam name="TModule">The module type.</typeparam>
    /// <typeparam name="TContext">The module's DbContext, which owns its outbox and inbox tables.</typeparam>
    /// <param name="configure">Declares whether the module publishes through an outbox and which events it consumes.</param>
    public static IServiceCollection AddMessaging<TModule, TContext>(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<IModuleMessagingBuilder<TModule, TContext>> configure
    )
        where TModule : IModule
        where TContext : DbContext
    {
        services.AddRabbitMq(configuration);

        configure(new ModuleMessagingBuilder<TModule, TContext>(services));

        return services;
    }
}
