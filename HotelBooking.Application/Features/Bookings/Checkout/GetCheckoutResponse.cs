using HotelBooking.Domain.Enums;

namespace HotelBooking.Application.Features.Bookings.Checkout;

public sealed class GetCheckoutResponse
{
    public int BookingId { get; set; }

    public CustomerInfo Customer { get; set; } = null!;

    public BookingSummary Summary { get; set; } = null!;

    public BookingCalculation Calculation { get; set; } = null!;
}

public sealed class CustomerInfo
{
    public int UserId { get; set; }

    public string Email { get; set; } = string.Empty;
}

public sealed class BookingSummary
{
    public string HotelName { get; set; } = string.Empty;

    public string CityName { get; set; } = string.Empty;

    public int RoomId { get; set; }

    public string RoomNumber { get; set; } = string.Empty;

    public DateTime CheckInDate { get; set; }

    public DateTime CheckOutDate { get; set; }

    public int Nights { get; set; }

    public BookingStatus Status { get; set; }
}

public sealed class BookingCalculation
{
    public decimal PricePerNight { get; set; }

    public int Nights { get; set; }

    public decimal TotalPrice { get; set; }
}