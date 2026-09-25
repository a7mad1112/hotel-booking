namespace HotelBooking.Application.Features.Amenities.UpdateAmenity;

public sealed class UpdateAmenityResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
