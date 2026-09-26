using HotelBooking.Application.Common.Images;
using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Application.Common.Results;
using HotelBooking.Domain.Entities;

namespace HotelBooking.Application.Features.Hotels.UploadHotelImage;

public sealed class UploadHotelImageService : IScopedService
{
    private readonly IHotelRepository _hotelRepository;
    private readonly IRepository<HotelImage> _hotelImageRepository;
    private readonly IImageService _imageService;
    private readonly ICurrentUserService? _currentUserService;

    public UploadHotelImageService(
        IHotelRepository hotelRepository,
        IRepository<HotelImage> hotelImageRepository,
        IImageService imageService,
        ICurrentUserService? currentUserService = null)
    {
        _hotelRepository = hotelRepository;
        _hotelImageRepository = hotelImageRepository;
        _imageService = imageService;
        _currentUserService = currentUserService;
    }

    public Task<ResultOfT<UploadHotelImageResponse>> UploadAsync(
        int hotelId,
        ImageUpload image,
        CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService?.UserId ?? 0;
        var isAdmin = _currentUserService?.IsAdmin ?? false;
        return UploadAsync(hotelId, image, currentUserId, isAdmin, cancellationToken);
    }

    public async Task<ResultOfT<UploadHotelImageResponse>> UploadAsync(
        int hotelId,
        ImageUpload image,
        int currentUserId,
        bool isAdmin,
        CancellationToken cancellationToken)
    {
        var hotel = await _hotelRepository.GetByIdAsync(hotelId, cancellationToken);

        if (hotel is null)
        {
            return ResultOfT<UploadHotelImageResponse>.Failure("Hotel not found.");
        }

        if (!isAdmin && hotel.OwnerId != currentUserId)
        {
            return ResultOfT<UploadHotelImageResponse>.Failure("You are not allowed to upload images for this hotel.");
        }

        var uploadResult = await _imageService.UploadAsync(image, ImageFolders.Hotels, cancellationToken);

        var hotelImage = new HotelImage
        {
            HotelId = hotelId,
            ImageUrl = uploadResult.Url,
            PublicId = uploadResult.PublicId
        };

        try
        {
            await _hotelImageRepository.AddAsync(hotelImage, cancellationToken);

            await _hotelImageRepository.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            try
            {
                await _imageService.DeleteAsync(uploadResult.PublicId, CancellationToken.None);
            }
            catch
            {
                // original database exception.
            }

            throw;
        }

        return ResultOfT<UploadHotelImageResponse>.Success(
            new UploadHotelImageResponse
            {
                Id = hotelImage.Id,
                HotelId = hotelImage.HotelId,
                ImageUrl = hotelImage.ImageUrl
            });
    }
}