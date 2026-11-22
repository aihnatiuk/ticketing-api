using BuildingBlocks.Contracts;

namespace Booking.Application;

/// <summary>
/// Represents the Booking module in the application.
/// Currently, a marker class, but may implement in-process cross-module API in the future.
/// </summary>
public sealed class BookingModule : IModule
{
    public static string ModuleName => "booking";
}
