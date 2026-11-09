using BuildingBlocks.Application;
using Microsoft.Extensions.DependencyInjection;

namespace Catalog.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddCatalogApplication(this IServiceCollection services) =>
        services.AddHandlersFromAssembly(typeof(CatalogModule).Assembly);
}
