using System.Linq.Expressions;
using HotelBooking.Application.Features.Search;
using HotelBooking.Application.Features.Search.Hotels;
using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Infrastructure.Persistence.Repositories.Search;

/// <summary>
/// Fluent query builder for building complex hotel search queries with multiple filtering criteria.
/// </summary>
public sealed class HotelSearchQueryBuilder
{
    private IQueryable<Hotel> _query;

    public HotelSearchQueryBuilder(IQueryable<Hotel> baseQuery)
    {
        _query = baseQuery;
    }

    public static HotelSearchQueryBuilder Create(IQueryable<Hotel> baseQuery) => new(baseQuery);

    public HotelSearchQueryBuilder WithSearchTerm(string? searchTerm)
    {
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim();
            _query = _query.Where(x =>
                EF.Functions.ILike(x.Name, $"%{term}%") ||
                EF.Functions.ILike(x.City.Name, $"%{term}%") ||
                EF.Functions.ILike(x.Location, $"%{term}%"));
        }

        return this;
    }

    public HotelSearchQueryBuilder WithCity(int? cityId)
    {
        if (cityId.HasValue)
        {
            _query = _query.Where(x => x.CityId == cityId.Value);
        }

        return this;
    }

    public HotelSearchQueryBuilder WithStarRating(decimal? minStarRating, decimal? maxStarRating)
    {
        if (minStarRating.HasValue)
        {
            _query = _query.Where(x => x.StarRating >= minStarRating.Value);
        }

        if (maxStarRating.HasValue)
        {
            _query = _query.Where(x => x.StarRating <= maxStarRating.Value);
        }

        return this;
    }

    public HotelSearchQueryBuilder WithAmenities(IReadOnlyCollection<int>? amenityIds)
    {
        if (amenityIds != null && amenityIds.Count > 0)
        {
            _query = _query.Where(h =>
                amenityIds.All(amenityId =>
                    h.HotelAmenities.Any(ha => ha.AmenityId == amenityId)));
        }

        return this;
    }

    public HotelSearchQueryBuilder WithRoomCriteria(SearchHotelsRequest request)
    {
        var rooms = _query.SelectMany(h => h.Rooms).Where(room => room.Availability);

        if (request.RoomTypeId.HasValue)
        {
            rooms = rooms.Where(room => room.RoomTypeId == request.RoomTypeId.Value);
        }

        if (request.Adults.HasValue)
        {
            rooms = rooms.Where(room => room.AdultsCapacity >= request.Adults.Value);
        }

        if (request.Children.HasValue)
        {
            rooms = rooms.Where(room => room.ChildrenCapacity >= request.Children.Value);
        }

        if (request.MinPrice.HasValue)
        {
            rooms = rooms.Where(room => room.PricePerNight >= request.MinPrice.Value);
        }

        if (request.MaxPrice.HasValue)
        {
            rooms = rooms.Where(room => room.PricePerNight <= request.MaxPrice.Value);
        }

        if (request.CheckInDate.HasValue && request.CheckOutDate.HasValue)
        {
            var checkIn = request.CheckInDate.Value;
            var checkOut = request.CheckOutDate.Value;

            rooms = rooms.Where(room =>
                !room.Bookings.Any(booking =>
                    (booking.Status == BookingStatus.Pending ||
                     booking.Status == BookingStatus.Confirmed)
                    &&
                    booking.CheckInDate < checkOut
                    &&
                    checkIn < booking.CheckOutDate));
        }

        if (request.Rooms.HasValue)
        {
            var requiredRooms = request.Rooms.Value;
            _query = _query.Where(h => rooms.Count(room => room.HotelId == h.Id) >= requiredRooms);
        }
        else if (HasAnyRoomFilter(request))
        {
            _query = _query.Where(h => rooms.Any(room => room.HotelId == h.Id));
        }

        return this;
    }

    public IQueryable<Hotel> Build() => _query;

    public static Expression<Func<Hotel, SearchHotelsResponse>> ProjectToResponse(SearchHotelsRequest request)
    {
        return hotel => new SearchHotelsResponse
        {
            Id = hotel.Id,
            Name = hotel.Name,
            Description = hotel.Description,
            StarRating = hotel.StarRating,
            Location = hotel.Location,
            CityId = hotel.CityId,
            CityName = hotel.City.Name,

            ThumbnailUrl = hotel.Images
                .OrderBy(image => image.Id)
                .Select(image => image.ImageUrl)
                .FirstOrDefault(),

            PricePerNight = hotel.Rooms
                .Where(room => room.Availability)
                .Where(room =>
                    !request.RoomTypeId.HasValue ||
                    room.RoomTypeId == request.RoomTypeId.Value)
                .Where(room =>
                    !request.Adults.HasValue ||
                    room.AdultsCapacity >= request.Adults.Value)
                .Where(room =>
                    !request.Children.HasValue ||
                    room.ChildrenCapacity >= request.Children.Value)
                .Where(room =>
                    !request.MinPrice.HasValue ||
                    room.PricePerNight >= request.MinPrice.Value)
                .Where(room =>
                    !request.MaxPrice.HasValue ||
                    room.PricePerNight <= request.MaxPrice.Value)
                .Where(room =>
                    !request.CheckInDate.HasValue ||
                    !request.CheckOutDate.HasValue ||
                    !room.Bookings.Any(booking =>
                        (booking.Status == BookingStatus.Pending ||
                         booking.Status == BookingStatus.Confirmed)
                        &&
                        booking.CheckInDate <
                        request.CheckOutDate.Value
                        &&
                        request.CheckInDate.Value <
                        booking.CheckOutDate))
                .Select(room => (decimal?)room.PricePerNight)
                .Min()
        };
    }

    private static bool HasAnyRoomFilter(SearchHotelsRequest request) =>
        request.RoomTypeId.HasValue ||
        request.Adults.HasValue ||
        request.Children.HasValue ||
        request.CheckInDate.HasValue ||
        request.CheckOutDate.HasValue ||
        request.MinPrice.HasValue ||
        request.MaxPrice.HasValue;
}
