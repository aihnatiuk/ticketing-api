using System.Text.Json.Serialization;

namespace Booking.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum OrderStatus
{
    Pending = 0,
    PaymentProcessing = 1,
    Paid = 2,
    Expired = 3,
    Canceled = 4,
}
