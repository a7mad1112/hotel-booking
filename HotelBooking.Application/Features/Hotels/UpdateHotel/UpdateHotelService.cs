using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Application.Common.Results;
using HotelBooking.Application.Features.Hotels;

namespace HotelBooking.Application.Features.Hotels.UpdateHotel;

public sealed class UpdateHotelService : IScopedService
{
    private readonly IHotelRepository _repository;

    public UpdateHotelService(
        IHotelRepository repository)
    {
        _repository = repository;
    }

    public async Task<ResultOfT<UpdateHotelResponse>> UpdateAsync(
        int id,
        UpdateHotelRequest request,
        int currentUserId,
        bool isAdmin,
        CancellationToken cancellationToken)
    {
        var hotel =
            await _repository.GetByIdAsync(
                id,
                cancellationToken);

        if (hotel is null)
        {
            return ResultOfT<UpdateHotelResponse>.Failure(
                "Hotel not found.");
        }

        if (!isAdmin && hotel.OwnerId != currentUserId)
        {
            return ResultOfT<UpdateHotelResponse>.Failure(
                "You are not allowed to update this hotel.");
        }

        var cityExists =
            await _repository.CityExistsAsync(
                request.CityId,
                cancellationToken);

        if (!cityExists)
        {
            return ResultOfT<UpdateHotelResponse>.Failure(
                "City not found.");
        }

        hotel.CityId = request.CityId;
        hotel.Name = request.Name.Trim();
        hotel.Description = request.Description;
        hotel.StarRating = request.StarRating;
        hotel.Location = request.Location.Trim();

        await _repository.SaveChangesAsync(
            cancellationToken);

        return ResultOfT<UpdateHotelResponse>.Success(
            new UpdateHotelResponse
            {
                Id = hotel.Id,
                CityId = hotel.CityId,
                Name = hotel.Name,
                Description = hotel.Description,
                StarRating = hotel.StarRating,
                Location = hotel.Location
            });
    }
}