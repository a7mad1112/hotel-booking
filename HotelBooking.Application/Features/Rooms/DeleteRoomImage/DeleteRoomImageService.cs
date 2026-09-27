using HotelBooking.Application.Common.Images;
using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Application.Common.Results;
using HotelBooking.Application.Features.Rooms;
using HotelBooking.Domain.Entities;

namespace HotelBooking.Application.Features.Rooms.DeleteRoomImage;

public sealed class DeleteRoomImageService : IScopedService
{
    private readonly IRoomRepository _roomRepository;
    private readonly IRepository<RoomImage> _roomImageRepository;
    private readonly IImageService _imageService;
    private readonly IImageDeletionOutboxRepository? _outboxRepository;

    public DeleteRoomImageService(
        IRoomRepository roomRepository,
        IRepository<RoomImage> roomImageRepository,
        IImageService imageService,
        IImageDeletionOutboxRepository? outboxRepository = null)
    {
        _roomRepository = roomRepository;
        _roomImageRepository = roomImageRepository;
        _imageService = imageService;
        _outboxRepository = outboxRepository;
    }

    public async Task<Result> DeleteAsync(int roomId, int imageId, int currentUserId, bool isAdmin,
        CancellationToken cancellationToken)
    {
        var room = await _roomRepository.GetDetailsByIdAsync(roomId, cancellationToken);

        if (room is null)
        {
            return Result.Failure("Room not found.");
        }

        if (!isAdmin && room.Hotel.OwnerId != currentUserId)
        {
            return Result.Failure("You are not allowed to delete images for this room.");
        }

        var image = await _roomImageRepository.GetByIdAsync(imageId, cancellationToken);

        if (image is null || image.RoomId != roomId)
        {
            return Result.Failure("Room image not found.");
        }

        if (_outboxRepository is not null)
        {
            await _outboxRepository.EnqueueRangeAsync([image.PublicId], cancellationToken);
        }

        _roomImageRepository.Delete(image);

        await _roomImageRepository.SaveChangesAsync(cancellationToken);

        try
        {
            await _imageService.DeleteAsync(image.PublicId, cancellationToken);
        }
        catch
        {
            // If direct deletion fails, background outbox worker will retry and clean it up.
        }

        return Result.Success();
    }
}