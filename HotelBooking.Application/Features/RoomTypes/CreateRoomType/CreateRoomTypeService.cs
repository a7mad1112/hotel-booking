using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Application.Common.Results;
using HotelBooking.Application.Features.RoomTypes;
using HotelBooking.Domain.Entities;

namespace HotelBooking.Application.Features.RoomTypes.CreateRoomType;

public sealed class CreateRoomTypeService : IScopedService
{
    private readonly IRoomTypeRepository _repository;

    public CreateRoomTypeService(IRoomTypeRepository repository)
    {
        _repository = repository;
    }

    public async Task<ResultOfT<CreateRoomTypeResponse>> CreateAsync(CreateRoomTypeRequest request,
        CancellationToken cancellationToken)
    {
        var roomType = new RoomType
        {
            Name = request.Name.Trim(),
            Description = request.Description?.Trim()
        };

        await _repository.AddAsync(roomType, cancellationToken);

        await _repository.SaveChangesAsync(cancellationToken);

        return ResultOfT<CreateRoomTypeResponse>.Success(
            new CreateRoomTypeResponse
            {
                Id = roomType.Id,
                Name = roomType.Name,
                Description = roomType.Description
            });
    }
}