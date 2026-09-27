using HotelBooking.Application.Common.Images;
using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Application.Common.Results;
using HotelBooking.Application.Features.Hotels;

namespace HotelBooking.Application.Features.Hotels.DeleteHotel;

public sealed class DeleteHotelService : IScopedService
{
    private readonly IHotelRepository _repository;
    private readonly ICurrentUserService? _currentUserService;
    private readonly IImageDeletionOutboxRepository? _outboxRepository;

    public DeleteHotelService(
        IHotelRepository repository,
        ICurrentUserService? currentUserService = null,
        IImageDeletionOutboxRepository? outboxRepository = null)
    {
        _repository = repository;
        _currentUserService = currentUserService;
        _outboxRepository = outboxRepository;
    }

    public Task<Result> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService?.UserId ?? 0;
        var isAdmin = _currentUserService?.IsAdmin ?? false;
        return DeleteAsync(id, currentUserId, isAdmin, cancellationToken);
    }

    public async Task<Result> DeleteAsync(int id, int currentUserId, bool isAdmin, CancellationToken cancellationToken)
    {
        var hotel =
            await _repository.GetByIdAsync(id, cancellationToken);

        if (hotel is null)
        {
            return Result.Failure("Hotel not found.");
        }

        if (!isAdmin && hotel.OwnerId != currentUserId)
        {
            return Result.Failure("You are not allowed to delete this hotel.");
        }

        var hasDependencies = await _repository.HasDependenciesAsync(id, cancellationToken);

        if (hasDependencies)
        {
            return Result.Failure("Cannot delete a hotel that has related data.");
        }

        var imagePublicIds = await _repository.GetImagePublicIdsAsync(id, cancellationToken);

        if (_outboxRepository is not null && imagePublicIds.Count > 0)
        {
            await _outboxRepository.EnqueueRangeAsync(imagePublicIds, cancellationToken);
        }

        _repository.Delete(hotel);

        await _repository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}