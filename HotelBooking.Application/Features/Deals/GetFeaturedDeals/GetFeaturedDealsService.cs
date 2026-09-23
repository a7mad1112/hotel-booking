using HotelBooking.Application.Common.Interfaces;

namespace HotelBooking.Application.Features.Deals.GetFeaturedDeals;

public sealed class GetFeaturedDealsService : IScopedService
{
    private const int FeaturedDealLimit = 5;

    private readonly IDealRepository _repository;

    public GetFeaturedDealsService(IDealRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<GetFeaturedDealsResponse>> GetAsync(CancellationToken cancellationToken)
    {
        var deals = await _repository.GetFeaturedAsync(DateTime.UtcNow, FeaturedDealLimit, cancellationToken);

        return deals.Select(deal =>
            {
                var hotel = deal.Hotel;

                var lowestAvailableRoomPrice = hotel.Rooms
                    .Where(room => room.Availability)
                    .Select(room => (decimal?)room.PricePerNight)
                    .Min();

                if (lowestAvailableRoomPrice is null)
                {
                    return null;
                }

                var originalPrice = lowestAvailableRoomPrice.Value;

                var discountedPrice = originalPrice * (1 - deal.DiscountPercentage / 100m);

                var imageUrl = hotel.Images
                    .OrderBy(image => image.Id)
                    .Select(image => image.ImageUrl)
                    .FirstOrDefault();

                return new GetFeaturedDealsResponse
                {
                    HotelId = hotel.Id,
                    HotelImageUrl = imageUrl,
                    HotelName = hotel.Name,
                    Location = hotel.Location,
                    OriginalPrice = originalPrice,
                    DiscountedPrice = decimal.Round(
                        discountedPrice,
                        2),
                    Rating = hotel.StarRating
                };
            })
            .Where(x => x is not null)
            .Select(x => x!)
            .ToList();
    }
}