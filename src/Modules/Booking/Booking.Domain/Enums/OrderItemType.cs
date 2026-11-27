using System.Text.Json.Serialization;

namespace Booking.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum OrderItemType
{
    /// <summary>
    /// Dedicated seat is a specific seat that is assigned to a customer.
    /// </summary>
    DedicatedSeat = 0,

    /// <summary>
    /// General admission is a type of ticket that allows access to the event without a specific seat assignment.
    /// </summary>
    GeneralAdmission = 1,
}
