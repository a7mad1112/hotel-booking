namespace HotelBooking.Application.Features.Amenities.UpdateAmenity;

public sealed class UpdateAmenityRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
