using HotelBooking.Application.Common.Images;
using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Application.Common.Results;
using HotelBooking.Application.Features.Rooms;
using HotelBooking.Domain.Entities;

namespace HotelBooking.Application.Features.Rooms.UploadRoomImage;

public sealed class UploadRoomImageService : IScopedService
{
    private readonly IRoomRepository _roomRepository;
    private readonly IRepository<RoomImage> _roomImageRepository;
    private readonly IImageService _imageService;

    public UploadRoomImageService(IRoomRepository roomRepository, IRepository<RoomImage> roomImageRepository,
        IImageService imageService)
    {
        _roomRepository = roomRepository;
        _roomImageRepository = roomImageRepository;
        _imageService = imageService;
    }

    public async Task<ResultOfT<UploadRoomImageResponse>> UploadAsync(int roomId, ImageUpload image, int currentUserId,
        bool isAdmin,
        CancellationToken cancellationToken)
    {
        var room = await _roomRepository.GetDetailsByIdAsync(roomId, cancellationToken);

        if (room is null)
        {
            return ResultOfT<UploadRoomImageResponse>.Failure("Room not found.");
        }

        if (!isAdmin && room.Hotel.OwnerId != currentUserId)
        {
            return ResultOfT<UploadRoomImageResponse>.Failure("You are not allowed to upload images for this room.");
        }

        var uploadResult = await _imageService.UploadAsync(image, ImageFolders.Rooms, cancellationToken);

        var roomImage = new RoomImage
        {
            RoomId = roomId,
            ImageUrl = uploadResult.Url,
            PublicId = uploadResult.PublicId
        };

        try
        {
            await _roomImageRepository.AddAsync(roomImage, cancellationToken);

            await _roomImageRepository.SaveChangesAsync(cancellationToken);
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

        return ResultOfT<UploadRoomImageResponse>.Success(
            new UploadRoomImageResponse
            {
                Id = roomImage.Id,
                RoomId = roomImage.RoomId,
                ImageUrl = roomImage.ImageUrl
            });
    }
}