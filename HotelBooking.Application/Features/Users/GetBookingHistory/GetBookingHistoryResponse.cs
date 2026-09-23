namespace HotelBooking.Application.Features.Users.GetBookingHistory;

public sealed class GetBookingHistoryResponse
{
    public int HotelId { get; init; }

    public string HotelName { get; init; } = string.Empty;

    public string CityName { get; init; } = string.Empty;

    public decimal StarRating { get; init; }

    public string? ImageUrl { get; init; }

    public decimal PricePerNight { get; init; }
}