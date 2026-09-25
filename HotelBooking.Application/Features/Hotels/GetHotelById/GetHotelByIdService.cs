using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Application.Common.Results;
using HotelBooking.Application.Features.Hotels;

namespace HotelBooking.Application.Features.Hotels.GetHotelById;

public sealed class GetHotelByIdService : IScopedService
{
    private readonly IHotelRepository _repository;

    public GetHotelByIdService(IHotelRepository repository)
    {
        _repository = repository;
    }

    public async Task<ResultOfT<GetHotelByIdResponse>> GetAsync(int id, CancellationToken cancellationToken)
    {
        var hotel = await _repository.GetDetailsByIdAsync(id, cancellationToken);

        if (hotel is null)
        {
            return ResultOfT<GetHotelByIdResponse>.Failure("Hotel not found.");
        }

        return ResultOfT<GetHotelByIdResponse>.Success(
            new GetHotelByIdResponse
            {
                Id = hotel.Id,
                Name = hotel.Name,
                Description = hotel.Description,
                StarRating = hotel.StarRating,
                Location = hotel.Location,

                CityId = hotel.CityId,
                CityName = hotel.City?.Name ?? string.Empty,
                Country = hotel.City?.Country ?? string.Empty,

                OwnerId = hotel.OwnerId,
                OwnerEmail = hotel.Owner?.Email ?? string.Empty,

                Images = hotel.Images
                    .Select(image => new HotelImageResponse
                    {
                        Id = image.Id,
                        ImageUrl = image.ImageUrl
                    })
                    .ToList(),

                Rooms = hotel.Rooms
                    .Select(room => new HotelRoomResponse
                    {
                        Id = room.Id,
                        RoomNumber = room.RoomNumber,

                        RoomTypeId = room.RoomTypeId,
                        RoomTypeName = room.RoomType?.Name ?? string.Empty,
                        RoomTypeDescription = room.RoomType?.Description ?? string.Empty,

                        PricePerNight = room.PricePerNight,
                        AdultsCapacity = room.AdultsCapacity,
                        ChildrenCapacity = room.ChildrenCapacity,
                        Availability = room.Availability,

                        Images = room.Images
                            .Select(image => new RoomImageResponse
                            {
                                Id = image.Id,
                                ImageUrl = image.ImageUrl
                            })
                            .ToList()
                    })
                    .ToList(),

                Reviews = hotel.Reviews
                    .Select(review => new HotelReviewResponse
                    {
                        Id = review.Id,
                        Rating = review.Rating,
                        Comment = review.Comment,
                        UserId = review.UserId
                    })
                    .ToList(),

                Amenities = hotel.HotelAmenities
                    .Where(ha => ha.Amenity != null)
                    .Select(ha => new HotelAmenityResponse
                    {
                        Id = ha.AmenityId,
                        Name = ha.Amenity.Name,
                        Description = ha.Amenity.Description
                    })
                    .ToList()
            });
    }
}