using Booking.Application;
using Booking.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Booking.Presentation;

public static class DependencyInjection
{
    public static IServiceCollection AddBookingModule(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddBookingApplication();
        services.AddBookingInfrastructure(configuration);

        return services;
    }
}
