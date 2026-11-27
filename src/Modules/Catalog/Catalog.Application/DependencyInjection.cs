using BuildingBlocks.Application;
using Catalog.Application.Features.Events.PublishEvent;
using Catalog.Application.Features.Venues.Common.Layout;
using Catalog.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Catalog.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddCatalogApplication(this IServiceCollection services)
    {
        services.AddHandlersFromAssembly(typeof(CatalogModule).Assembly);
        services.AddScoped<IVenueLayoutValidator, VenueLayoutValidator>();
        services.AddScoped<IEventPublishingReader, EventPublishingReader>();

        services.AddScoped<ICatalogModule, CatalogModule>();

        return services;
    }
}
