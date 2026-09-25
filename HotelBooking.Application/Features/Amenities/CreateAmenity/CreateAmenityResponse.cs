namespace HotelBooking.Application.Features.Amenities.CreateAmenity;

public sealed class CreateAmenityResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
