namespace HotelBooking.Application.Features.Amenities.CreateAmenity;

public sealed class CreateAmenityRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
