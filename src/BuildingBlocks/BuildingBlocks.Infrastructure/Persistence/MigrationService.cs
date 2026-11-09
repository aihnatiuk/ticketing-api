using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Infrastructure.Persistence;

internal sealed class MigrationService<TContext>(
    IServiceProvider serviceProvider,
    IHostEnvironment hostEnvironment,
    ILogger<MigrationService<TContext>> logger
) : IHostedService
    where TContext : DbContext
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (!hostEnvironment.IsDevelopment())
        {
            return;
        }

        string contextName = typeof(TContext).Name;
        logger.LogInformation("Applying {Context} migrations...", contextName);

        using IServiceScope scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TContext>();

        try
        {
            await context.Database.MigrateAsync(cancellationToken);
            logger.LogInformation("{Context} migrations applied.", contextName);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to apply {Context} migrations.", contextName);
            throw;
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
