namespace HotelBooking.Application.Features.Cities.CreateCity;

public sealed class CreateCityResponse
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;

    public string Country { get; init; } = string.Empty;
    public string? PostalCode { get; init; }
}