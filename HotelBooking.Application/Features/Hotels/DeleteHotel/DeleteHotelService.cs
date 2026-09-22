using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Application.Common.Results;
using HotelBooking.Application.Features.Hotels;

namespace HotelBooking.Application.Features.Hotels.DeleteHotel;

public sealed class DeleteHotelService : IScopedService
{
    private readonly IHotelRepository _repository;

    public DeleteHotelService(
        IHotelRepository repository)
    {
        _repository = repository;
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

        _repository.Delete(hotel);

        await _repository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}