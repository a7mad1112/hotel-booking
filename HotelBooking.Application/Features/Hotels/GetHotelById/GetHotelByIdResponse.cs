namespace HotelBooking.Application.Features.Hotels.GetHotelById;

public sealed class GetHotelByIdResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal StarRating { get; set; }
    public string Location { get; set; } = string.Empty;

    public int CityId { get; set; }
    public string CityName { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;

    public int OwnerId { get; set; }
    public string OwnerEmail { get; set; } = string.Empty;

    public List<HotelImageResponse> Images { get; set; } = [];
    public List<HotelRoomResponse> Rooms { get; set; } = [];
    public List<HotelReviewResponse> Reviews { get; set; } = [];
    public List<HotelAmenityResponse> Amenities { get; set; } = [];
}