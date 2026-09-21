namespace HotelBooking.Application.Features.Cities.UpdateCity;

public sealed class UpdateCityRequest
{
    public string Name { get; set; } = string.Empty;

    public string Country { get; set; } = string.Empty;

    public string? PostalCode { get; set; }
}