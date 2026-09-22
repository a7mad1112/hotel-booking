using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Application.Common.Results;
using HotelBooking.Domain.Entities;

namespace HotelBooking.Application.Features.Hotels.CreateHotel;

public sealed class CreateHotelService
    : IScopedService
{
    private readonly IHotelRepository _repository;


    public CreateHotelService(
        IHotelRepository repository)
    {
        _repository = repository;
    }


    public async Task<ResultOfT<CreateHotelResponse>> CreateAsync(
        CreateHotelRequest request,
        CancellationToken cancellationToken)
    {
        var cityExists =
            await _repository.CityExistsAsync(
                request.CityId,
                cancellationToken);


        if (!cityExists)
        {
            return ResultOfT<CreateHotelResponse>.Failure(
                "City not found.");
        }


        var ownerExists =
            await _repository.OwnerExistsAsync(
                request.OwnerId,
                cancellationToken);


        if (!ownerExists)
        {
            return ResultOfT<CreateHotelResponse>.Failure(
                "Owner not found.");
        }


        var hotel = new Hotel
        {
            CityId = request.CityId,
            OwnerId = request.OwnerId,
            Name = request.Name.Trim(),
            Description = request.Description?.Trim(),
            StarRating = request.StarRating,
            Location = request.Location.Trim()
        };


        await _repository.AddAsync(
            hotel,
            cancellationToken);


        await _repository.SaveChangesAsync(
            cancellationToken);


        return ResultOfT<CreateHotelResponse>.Success(
            new CreateHotelResponse
            {
                Id = hotel.Id,
                Name = hotel.Name
            });
    }
}