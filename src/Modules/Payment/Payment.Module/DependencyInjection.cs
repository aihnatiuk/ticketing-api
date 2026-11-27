using BuildingBlocks.Application;
using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Infrastructure.Messaging.Configuration;
using BuildingBlocks.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Payment.Contracts;
using Payment.Module.Persistence;
using Payment.Module.Persistence.Repositories;

namespace Payment.Module;

public static class DependencyInjection
{
    public static IServiceCollection AddPaymentModule(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddHandlersFromAssembly(typeof(PaymentModule).Assembly);
        services.AddScoped<IPaymentModule, PaymentModule>();

        services.AddPersistence(configuration);
        services.AddMessaging<PaymentModule, PaymentDbContext>(
            configuration,
            messaging => messaging.UseOutbox()
        );

        services.AddControllers().AddApplicationPart(typeof(DependencyInjection).Assembly);

        return services;
    }

    private static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var connectionString = configuration.GetConnectionString("Database");
        if (connectionString is null)
        {
            throw new InvalidOperationException("Connection string 'Database' is not configured.");
        }

        services.AddDbContext<PaymentDbContext>(options =>
            options.UseSqlServer(
                connectionString,
                sql => sql.MigrationsHistoryTable("__EFMigrationsHistory", PaymentDbContext.Schema)
            )
        );
        services.AddScoped<IUnitOfWork<PaymentModule>>(sp => new UnitOfWork<PaymentModule>(
            sp.GetRequiredService<PaymentDbContext>()
        ));
        services.AddScoped<IPaymentRepository, PaymentRepository>();

        services.AddMigrationService<PaymentDbContext>();

        return services;
    }
}
