using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Application.Common.Pagination;
using HotelBooking.Domain.Enums;

namespace HotelBooking.Application.Features.Users.GetUsers;

public sealed class GetUsersService : IScopedService
{
    private readonly IUserRepository _userRepository;

    public GetUsersService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<PagedResult<GetUsersResponse>> GetUsersAsync(
        PaginationRequest request,
        string? search,
        UserRole? role,
        CancellationToken cancellationToken)
    {
        var result = await _userRepository.GetPagedAsync(
            request.Page,
            request.PageSize,
            search,
            role,
            cancellationToken);

        return new PagedResult<GetUsersResponse>
        {
            Items = result.Items
                .Select(u => new GetUsersResponse
                {
                    Id = u.Id,
                    Email = u.Email,
                    Role = u.Role,
                    CreatedAt = u.CreatedAt
                })
                .ToList(),

            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = result.TotalCount
        };
    }
}
