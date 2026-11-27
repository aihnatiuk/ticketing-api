using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Infrastructure.Persistence;

public static class MigrationServiceExtensions
{
    /// <summary>
    /// Applies pending migrations of <typeparamref name="TContext"/> on startup (Development only).
    /// </summary>
    public static IServiceCollection AddMigrationService<TContext>(this IServiceCollection services)
        where TContext : DbContext => services.AddHostedService<MigrationService<TContext>>();
}
