namespace HotelBooking.Application.Features.Hotels.GetHotels;

public sealed class GetHotelsResponse
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public decimal StarRating { get; set; }

    public string Location { get; set; } = string.Empty;

    public int CityId { get; set; }

    public string CityName { get; set; } = string.Empty;

    public int OwnerId { get; set; }

    public string OwnerEmail { get; set; } = string.Empty;
}