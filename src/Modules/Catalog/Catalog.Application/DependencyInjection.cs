using BuildingBlocks.Application;
using Catalog.Application.Features.Venues.Common.Layout;
using Microsoft.Extensions.DependencyInjection;

namespace Catalog.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddCatalogApplication(this IServiceCollection services)
    {
        services.AddHandlersFromAssembly(typeof(CatalogModule).Assembly);
        services.AddScoped<IVenueLayoutValidator, VenueLayoutValidator>();

        return services;
    }
}
