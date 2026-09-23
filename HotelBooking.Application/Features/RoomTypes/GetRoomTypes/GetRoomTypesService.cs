using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Application.Common.Pagination;
using HotelBooking.Application.Features.RoomTypes;

namespace HotelBooking.Application.Features.RoomTypes.GetRoomTypes;

public sealed class GetRoomTypesService : IScopedService
{
    private readonly IRoomTypeRepository _repository;

    public GetRoomTypesService(IRoomTypeRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResult<GetRoomTypesResponse>> GetAllAsync(PaginationRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _repository.GetPagedAsync(
            request.Page,
            request.PageSize,
            cancellationToken);

        return new PagedResult<GetRoomTypesResponse>
        {
            Items = result.Items
                .Select(roomType => new GetRoomTypesResponse
                {
                    Id = roomType.Id,
                    Name = roomType.Name,
                    Description = roomType.Description
                })
                .ToList(),

            Page = request.Page,

            PageSize = request.PageSize,

            TotalCount = result.TotalCount
        };
    }
}