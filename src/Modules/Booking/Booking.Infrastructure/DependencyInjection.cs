using Booking.Application;
using Booking.Application.Abstractions;
using Booking.Infrastructure.Persistence;
using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Booking.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddBookingInfrastructure(
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
        var connectionString =
            configuration.GetConnectionString("Database")
            ?? throw new InvalidOperationException(
                "Connection string 'Database' is not configured."
            );

        services.AddDbContext<BookingDbContext>(options =>
            options.UseSqlServer(
                connectionString,
                sql => sql.MigrationsHistoryTable("__EFMigrationsHistory", BookingDbContext.Schema)
            )
        );
        services.AddScoped<IBookingDbContext>(sp => sp.GetRequiredService<BookingDbContext>());
        services.AddScoped<IUnitOfWork<BookingModule>>(sp =>
            sp.GetRequiredService<BookingDbContext>()
        );

        services.AddMigrationService<BookingDbContext>();

        return services;
    }
}
