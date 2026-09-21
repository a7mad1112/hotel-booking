namespace HotelBooking.Application.Features.Cities.GetCityById;

public sealed class GetCityByIdResponse
{
    public int Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string Country { get; init; } = string.Empty;

    public string? PostalCode { get; init; }
}