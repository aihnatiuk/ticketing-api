using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Infrastructure.Persistence;
using Catalog.Application;
using Catalog.Application.Abstractions;
using Catalog.Infrastructure.Persistence;
using Catalog.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Catalog.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddCatalogInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddPersistence(configuration);

        return services;
    }

    private static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        string? connectionString = configuration.GetConnectionString("Database");
        if (connectionString is null)
        {
            throw new InvalidOperationException("Connection string 'Database' is not configured.");
        }

        services.AddDbContext<CatalogDbContext>(options =>
            options.UseSqlServer(
                connectionString,
                sql => sql.MigrationsHistoryTable("__EFMigrationsHistory", CatalogDbContext.Schema)
            )
        );
        services.AddScoped<ICatalogDbContext>(sp => sp.GetRequiredService<CatalogDbContext>());
        services.AddScoped<IUnitOfWork<CatalogModule>>(sp =>
            sp.GetRequiredService<CatalogDbContext>()
        );

        services.AddScoped<IVenueRepository, VenueRepository>();

        services.AddMigrationService<CatalogDbContext>();

        return services;
    }
}
