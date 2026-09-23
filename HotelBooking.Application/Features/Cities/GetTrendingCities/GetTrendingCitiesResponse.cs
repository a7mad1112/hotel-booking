namespace HotelBooking.Application.Features.Cities.GetTrendingCities;

public sealed class GetTrendingCitiesResponse
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;
    public int BookingCount { get; init; }
}