using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Application.Features.Search;
using HotelBooking.Application.Features.Search.Hotels;
using HotelBooking.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Infrastructure.Persistence.Repositories;

public sealed class HotelSearchRepository : IHotelSearchRepository, IScopedService
{
    private readonly ApplicationDbContext _context;

    public HotelSearchRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<(List<SearchHotelsResponse> Items, int TotalCount)> SearchAsync(
        SearchHotelsRequest request,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var hotels = _context.Hotels.AsNoTracking().AsQueryable();

        if (request.CityId.HasValue)
        {
            hotels = hotels.Where(x => x.CityId == request.CityId.Value);
        }

        if (request.MinStarRating.HasValue)
        {
            hotels = hotels.Where(x => x.StarRating >= request.MinStarRating.Value);
        }

        if (request.MaxStarRating.HasValue)
        {
            hotels = hotels.Where(x => x.StarRating <= request.MaxStarRating.Value);
        }

        if (request.AmenityIds.Count > 0)
        {
            hotels = hotels.Where(h =>
                request.AmenityIds.All(amenityId =>
                    h.HotelAmenities.Any(ha => ha.AmenityId == amenityId)));
        }

        var rooms = hotels.SelectMany(h => h.Rooms).Where(room => room.Availability);

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

            hotels = hotels.Where(h => rooms.Count(room => room.HotelId == h.Id) >= requiredRooms);
        }
        else if (
            request.RoomTypeId.HasValue ||
            request.Adults.HasValue ||
            request.Children.HasValue ||
            request.CheckInDate.HasValue ||
            request.CheckOutDate.HasValue ||
            request.MinPrice.HasValue ||
            request.MaxPrice.HasValue)
        {
            hotels = hotels.Where(h => rooms.Any(room => room.HotelId == h.Id));
        }

        var totalCount = await hotels.CountAsync(cancellationToken);

        var items =
            await hotels
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(hotel => new SearchHotelsResponse
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
                })
                .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}