namespace HotelBooking.Application.Features.Search.Hotels;

public sealed class SearchHotelsRequest
{
    public string? SearchTerm { get; set; }
    public int? CityId { get; set; }
    public DateTime? CheckInDate { get; set; }
    public DateTime? CheckOutDate { get; set; }
    public int? Adults { get; set; }
    public int? Children { get; set; }
    public int? Rooms { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public decimal? MinStarRating { get; set; }
    public decimal? MaxStarRating { get; set; }
    public List<int> AmenityIds { get; set; } = [];
    public int? RoomTypeId { get; set; }
}