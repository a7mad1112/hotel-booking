namespace HotelBooking.Application.Features.Cities.CreateCity;

public sealed class CreateCityRequest
{
    public string Name { get; set; } = string.Empty;

    public string Country { get; set; } = string.Empty;

    public string? PostalCode { get; set; }
}