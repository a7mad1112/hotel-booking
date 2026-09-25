namespace HotelBooking.Application.Features.Amenities.GetAmenities;

public sealed class GetAmenitiesResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
