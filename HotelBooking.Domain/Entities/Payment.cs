using System.Numerics;
using HotelBooking.Domain.Common;
using HotelBooking.Domain.Enums;

namespace HotelBooking.Domain.Entities;

public class Payment : BaseEntity
{
    public int BookingId { get; set; }
    public string Provider { get; set; }
    public string TransactionId { get; set; } = null;
    public decimal Amount { get; set; }
    public PaymentStatus Status { get; set; }
    public DateTimeOffset PaymentDate { get; set; }

    public Booking Booking { get; set; }
}