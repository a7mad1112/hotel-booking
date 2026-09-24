using HotelBooking.Application.Common.Pagination;
using HotelBooking.Application.Features.Search.Hotels;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.API.Controllers;

[ApiController]
[Route("api/search")]
public class SearchController : ControllerBase
{
    private readonly SearchHotelsService _searchHotelsService;

    public SearchController(SearchHotelsService searchHotelsService)
    {
        _searchHotelsService = searchHotelsService;
    }

    [HttpGet("hotels")]
    public async Task<ActionResult<PagedResult<SearchHotelsResponse>>> SearchHotels(
        [FromQuery] SearchHotelsRequest request,
        [FromQuery] PaginationRequest pagination,
        CancellationToken cancellationToken)
    {
        var result = await _searchHotelsService.SearchAsync(request, pagination, cancellationToken);

        return Ok(result);
    }
}