namespace HotelBooking.Application.Features.Hotels.CreateHotel;

public sealed class CreateHotelRequest
{
    public int CityId { get; set; }

    public int OwnerId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public decimal StarRating { get; set; }

    public string Location { get; set; } = string.Empty;
}