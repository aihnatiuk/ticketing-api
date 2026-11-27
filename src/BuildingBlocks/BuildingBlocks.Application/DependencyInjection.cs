using System.Reflection;
using BuildingBlocks.Application.Abstractions;
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
}
