using System.Text.Json.Serialization;

namespace Booking.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SeatStatus
{
    Available = 0,
    Reserved = 1,
    Sold = 2,
}
