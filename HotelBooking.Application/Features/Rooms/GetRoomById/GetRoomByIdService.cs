using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Application.Common.Results;
using HotelBooking.Application.Features.Rooms;

namespace HotelBooking.Application.Features.Rooms.GetRoomById;

public sealed class GetRoomByIdService : IScopedService
{
    private readonly IRoomRepository _repository;

    public GetRoomByIdService(IRoomRepository repository)
    {
        _repository = repository;
    }

    public async Task<ResultOfT<GetRoomByIdResponse>> GetAsync(int id, CancellationToken cancellationToken)
    {
        var room = await _repository.GetDetailsByIdAsync(id, cancellationToken);

        if (room is null)
        {
            return ResultOfT<GetRoomByIdResponse>.Failure("Room not found.");
        }

        return ResultOfT<GetRoomByIdResponse>.Success(
            new GetRoomByIdResponse
            {
                Id = room.Id,
                HotelId = room.HotelId,
                HotelName = room.Hotel.Name,
                RoomTypeId = room.RoomTypeId,
                RoomTypeName = room.RoomType.Name,
                RoomNumber = room.RoomNumber,
                PricePerNight = room.PricePerNight,
                AdultsCapacity = room.AdultsCapacity,
                ChildrenCapacity = room.ChildrenCapacity,
                Availability = room.Availability
            });
    }
}