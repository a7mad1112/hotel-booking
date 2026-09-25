namespace HotelBooking.Application.Features.Cities.GetCities;

public sealed class GetCitiesResponse
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;
    public string? PostalCode { get; init; }
    public int NumberOfHotels { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset UpdatedAt { get; init; }
}