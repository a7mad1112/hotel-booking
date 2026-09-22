using HotelBooking.Domain.Common;
using HotelBooking.Domain.Enums;

namespace HotelBooking.Domain.Entities;

public class Payment : AuditableEntity
{
    public int BookingId { get; set; }
    public required string Provider { get; set; }
    public required string TransactionId { get; set; }
    public decimal Amount { get; set; }
    public PaymentStatus Status { get; set; }
    public DateTimeOffset PaymentDate { get; set; }

    public Booking Booking { get; set; } = null!;
}