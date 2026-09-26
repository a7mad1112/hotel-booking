namespace HotelBooking.Application.Common.Pagination;

public sealed class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; init; } = [];

    public int Page { get; init; }

    public int PageSize { get; init; }

    public int TotalCount { get; init; }

    public int TotalPages =>
        (int)Math.Ceiling(
            TotalCount / (double)PageSize);

    public static PagedResult<T> Create(
        IReadOnlyList<T> items,
        int totalCount,
        int page,
        int pageSize)
    {
        return new PagedResult<T>
        {
            Items = items ?? [],
            TotalCount = totalCount,
            Page = page < 1 ? 1 : page,
            PageSize = pageSize < 1 ? 10 : pageSize
        };
    }

    public static PagedResult<T> Create(
        IReadOnlyList<T> items,
        int totalCount,
        PaginationRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        return Create(items, totalCount, request.Page, request.PageSize);
    }
}