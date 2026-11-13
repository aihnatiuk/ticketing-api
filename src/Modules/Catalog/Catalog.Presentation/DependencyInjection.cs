using Catalog.Application;
using Catalog.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Catalog.Presentation;

public static class DependencyInjection
{
    public static IServiceCollection AddCatalogModule(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddCatalogApplication();
        services.AddCatalogInfrastructure(configuration);

        services.AddControllers().AddApplicationPart(typeof(DependencyInjection).Assembly);

        return services;
    }
}
