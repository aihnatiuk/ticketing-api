using Booking.Application;
using Booking.Application.Abstractions;
using Booking.Infrastructure.Persistence;
using Booking.Infrastructure.Persistence.Repositories;
using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Infrastructure.Messaging.Configuration;
using BuildingBlocks.Infrastructure.Persistence;
using Catalog.Contracts.IntegrationEvents;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Payment.Contracts.IntegrationEvents;

namespace Booking.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddBookingInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddPersistence(configuration);
        services.AddMessaging<BookingModule, BookingDbContext>(
            configuration,
            messaging =>
                messaging
                    .Subscribe<EventPublishedIntegrationEvent>()
                    .Subscribe<PaymentSucceededIntegrationEvent>()
        );

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

        services.AddDbContext<BookingDbContext>(options =>
            options.UseSqlServer(
                connectionString,
                sql => sql.MigrationsHistoryTable("__EFMigrationsHistory", BookingDbContext.Schema)
            )
        );
        services.AddScoped<IBookingDbContext>(sp => sp.GetRequiredService<BookingDbContext>());
        services.AddScoped<IUnitOfWork<BookingModule>>(sp => new UnitOfWork<BookingModule>(
            sp.GetRequiredService<BookingDbContext>()
        ));

        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<ISeatRepository, SeatRepository>();

        services.AddMigrationService<BookingDbContext>();

        return services;
    }
}
