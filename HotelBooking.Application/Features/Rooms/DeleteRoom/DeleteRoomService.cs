using HotelBooking.Application.Common.Images;
using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Application.Common.Results;

namespace HotelBooking.Application.Features.Rooms.DeleteRoom;

public sealed class DeleteRoomService : IScopedService
{
    private readonly IRoomRepository _repository;
    private readonly IImageDeletionOutboxRepository? _outboxRepository;

    public DeleteRoomService(
        IRoomRepository repository,
        IImageDeletionOutboxRepository? outboxRepository = null)
    {
        _repository = repository;
        _outboxRepository = outboxRepository;
    }

    public async Task<Result> DeleteAsync(int id, int currentUserId, bool isAdmin, CancellationToken cancellationToken)
    {
        var room = await _repository.GetDetailsByIdAsync(id, cancellationToken);

        if (room is null)
        {
            return Result.Failure("Room not found.");
        }

        if (!isAdmin && room.Hotel.OwnerId != currentUserId)
        {
            return Result.Failure("You are not allowed to delete this room.");
        }

        var hasBookings = await _repository.HasBookingsAsync(id, cancellationToken);

        if (hasBookings)
        {
            return Result.Failure("Cannot delete a room that has related bookings.");
        }

        var imagePublicIds = await _repository.GetImagePublicIdsAsync(id, cancellationToken);

        if (_outboxRepository is not null && imagePublicIds.Count > 0)
        {
            await _outboxRepository.EnqueueRangeAsync(imagePublicIds, cancellationToken);
        }

        _repository.Delete(room);

        await _repository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}