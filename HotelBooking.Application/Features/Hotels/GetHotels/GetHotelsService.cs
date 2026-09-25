using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Application.Common.Pagination;
using HotelBooking.Application.Features.Hotels;

namespace HotelBooking.Application.Features.Hotels.GetHotels;

public sealed class GetHotelsService : IScopedService
{
    private readonly IHotelRepository _repository;

    public GetHotelsService(IHotelRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResult<GetHotelsResponse>> GetAllAsync(
        PaginationRequest request,
        string? search,
        CancellationToken cancellationToken)
    {
        var result = await _repository.GetPagedAsync(
            request.Page,
            request.PageSize,
            search,
            cancellationToken);

        return new PagedResult<GetHotelsResponse>
        {
            Items = result.Items
                .Select(hotel => new GetHotelsResponse
                {
                    Id = hotel.Id,
                    Name = hotel.Name,
                    Description = hotel.Description,
                    StarRating = hotel.StarRating,
                    Location = hotel.Location,
                    CityId = hotel.CityId,
                    CityName = hotel.City.Name,
                    OwnerId = hotel.OwnerId,
                    OwnerEmail = hotel.Owner.Email,
                    NumberOfRooms = hotel.Rooms?.Count ?? 0,
                    CreatedAt = hotel.CreatedAt,
                    UpdatedAt = hotel.UpdatedAt
                })
                .ToList(),

            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = result.TotalCount
        };
    }
}