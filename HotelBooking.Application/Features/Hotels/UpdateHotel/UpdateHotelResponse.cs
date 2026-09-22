namespace HotelBooking.Application.Features.Hotels.UpdateHotel;

public sealed class UpdateHotelResponse
{
    public int Id { get; set; }

    public int CityId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public decimal StarRating { get; set; }

    public string Location { get; set; } = string.Empty;
}