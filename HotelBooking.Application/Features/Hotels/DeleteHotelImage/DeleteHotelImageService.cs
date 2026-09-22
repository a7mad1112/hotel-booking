using HotelBooking.Application.Common.Images;
using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Application.Common.Results;
using HotelBooking.Application.Features.Hotels;
using HotelBooking.Domain.Entities;

namespace HotelBooking.Application.Features.Hotels.DeleteHotelImage;

public sealed class DeleteHotelImageService : IScopedService
{
    private readonly IHotelRepository _hotelRepository;
    private readonly IRepository<HotelImage> _hotelImageRepository;
    private readonly IImageService _imageService;

    public DeleteHotelImageService(
        IHotelRepository hotelRepository,
        IRepository<HotelImage> hotelImageRepository,
        IImageService imageService)
    {
        _hotelRepository = hotelRepository;
        _hotelImageRepository = hotelImageRepository;
        _imageService = imageService;
    }

    public async Task<Result> DeleteAsync(
        int hotelId,
        int imageId,
        int currentUserId,
        bool isAdmin,
        CancellationToken cancellationToken)
    {
        var hotel = await _hotelRepository.GetByIdAsync(hotelId, cancellationToken);

        if (hotel is null)
        {
            return Result.Failure("Hotel not found.");
        }

        if (!isAdmin && hotel.OwnerId != currentUserId)
        {
            return Result.Failure("You are not allowed to delete images for this hotel.");
        }

        var image = await _hotelImageRepository.GetByIdAsync(imageId, cancellationToken);

        if (image is null || image.HotelId != hotelId)
        {
            return Result.Failure("Hotel image not found.");
        }

        _hotelImageRepository.Delete(image);

        await _hotelImageRepository.SaveChangesAsync(cancellationToken);

        await _imageService.DeleteAsync(image.PublicId, cancellationToken);

        return Result.Success();
    }
}