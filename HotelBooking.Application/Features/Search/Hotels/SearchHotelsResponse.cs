namespace HotelBooking.Application.Features.Search.Hotels;

public sealed class SearchHotelsResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal StarRating { get; set; }
    public string Location { get; set; } = string.Empty;
    public int CityId { get; set; }
    public string CityName { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public decimal? PricePerNight { get; set; }
}