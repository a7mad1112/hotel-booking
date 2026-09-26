using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Application.Features.Search;
using HotelBooking.Application.Features.Search.Hotels;
using HotelBooking.Infrastructure.Persistence.Repositories.Search;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Infrastructure.Persistence.Repositories;

public sealed class HotelSearchRepository : IHotelSearchRepository, IScopedService
{
    private readonly ApplicationDbContext _context;

    public HotelSearchRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<(List<SearchHotelsResponse> Items, int TotalCount)> SearchAsync(
        SearchHotelsRequest request,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var query = HotelSearchQueryBuilder
            .Create(_context.Hotels.AsNoTracking())
            .WithSearchTerm(request.SearchTerm)
            .WithCity(request.CityId)
            .WithStarRating(request.MinStarRating, request.MaxStarRating)
            .WithAmenities(request.AmenityIds)
            .WithRoomCriteria(request)
            .Build();

        var totalCount = await query.CountAsync(cancellationToken);

        var safePage = page < 1 ? 1 : page;
        var safePageSize = pageSize < 1 ? 10 : pageSize;

        var items = await query
            .OrderBy(x => x.Name)
            .ThenBy(x => x.Id)
            .Skip((safePage - 1) * safePageSize)
            .Take(safePageSize)
            .Select(HotelSearchQueryBuilder.ProjectToResponse(request))
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}