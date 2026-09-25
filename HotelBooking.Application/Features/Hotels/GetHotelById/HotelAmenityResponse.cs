namespace HotelBooking.Application.Features.Hotels.GetHotelById;

public sealed class HotelAmenityResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
