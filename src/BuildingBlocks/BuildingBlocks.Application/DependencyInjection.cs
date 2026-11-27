using System.Reflection;
using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Application;

public static class DependencyInjection
{
    private static readonly Type[] HandlerInterfaces =
    [
        typeof(ICommandHandler<>),
        typeof(ICommandHandler<,>),
        typeof(IQueryHandler<,>),
    ];

    /// <summary>
    /// Registers every command/query handler found in the assembly as scoped.
    /// </summary>
    public static IServiceCollection AddHandlersFromAssembly(
        this IServiceCollection services,
        Assembly assembly
    )
    {
        var handlers = assembly
            .GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false })
            .SelectMany(t =>
                t.GetInterfaces()
                    .Where(i =>
                        i.IsGenericType && HandlerInterfaces.Contains(i.GetGenericTypeDefinition())
                    )
                    .Select(i => (Service: i, Implementation: t))
            );

        foreach (var (service, implementation) in handlers)
        {
            services.AddScoped(service, implementation);
        }

        return services;
    }

    /// <summary>
    /// Registers every integration event handler found in the module's assembly as a scoped service
    /// keyed by <typeparamref name="TModule"/>, so several modules can react to the same event.
    /// </summary>
    /// <typeparam name="TModule">The module type. Must live in the assembly that contains the handlers.</typeparam>
    public static IServiceCollection AddIntegrationEventHandlers<TModule>(
        this IServiceCollection services
    )
        where TModule : IModule
    {
        Type moduleType = typeof(TModule);

        var handlers = moduleType
            .Assembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false })
            .SelectMany(t =>
                t.GetInterfaces()
                    .Where(i =>
                        i.IsGenericType
                        && i.GetGenericTypeDefinition() == typeof(IIntegrationEventHandler<>)
                    )
                    .Select(i => (Service: i, Implementation: t))
            );

        foreach (var (service, implementation) in handlers)
        {
            services.AddKeyedScoped(service, serviceKey: moduleType, implementation);
        }

        return services;
    }
}
