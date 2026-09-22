namespace HotelBooking.Application.Features.Cities.UpdateCity;

public sealed class UpdateCityResponse
{
    public int Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string Country { get; init; } = string.Empty;

    public string? PostalCode { get; init; }
}