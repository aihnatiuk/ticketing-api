namespace Booking.Domain.Enums;

public enum OrderStatus
{
    Pending = 0,
    PaymentProcessing = 1,
    Paid = 2,
    Expired = 3,
    Canceled = 4,
}
