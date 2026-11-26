using Booking.Application.Features.Orders.Common.Services;
using BuildingBlocks.Application;
using Microsoft.Extensions.DependencyInjection;

namespace Booking.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddBookingApplication(this IServiceCollection services)
    {
        services.AddHandlersFromAssembly(typeof(BookingModule).Assembly);
        services.AddScoped<IOrderingService, OrderingService>();
        services.AddIntegrationEventHandlers<BookingModule>();

        return services;
    }
}
