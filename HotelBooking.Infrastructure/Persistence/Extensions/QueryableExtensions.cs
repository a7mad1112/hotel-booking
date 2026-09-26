using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Infrastructure.Persistence.Extensions;

public static class QueryableExtensions
{
    /// <summary>
    /// Executes a count query and applies pagination asynchronously.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="query">The source query.</param>
    /// <param name="page">1-based page number.</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A tuple containing the paged items list and total count.</returns>
    public static async Task<(List<T> Items, int TotalCount)> ToPagedListAsync<T>(
        this IQueryable<T> query,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var totalCount = await query.CountAsync(cancellationToken);

        var safePage = page < 1 ? 1 : page;
        var safePageSize = pageSize < 1 ? 10 : pageSize;

        var items = await query
            .Skip((safePage - 1) * safePageSize)
            .Take(safePageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}
