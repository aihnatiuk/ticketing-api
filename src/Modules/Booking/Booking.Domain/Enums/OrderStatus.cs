using System.Text.Json.Serialization;

namespace Booking.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum OrderStatus
{
    Pending = 0,
    PaymentProcessing = 1,
    Paid = 2,
    PaymentFailed = 3,
    Expired = 4,
    Canceled = 5,
}
