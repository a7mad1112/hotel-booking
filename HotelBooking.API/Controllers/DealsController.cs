using System.Security.Claims;
using HotelBooking.API.Authorization;
using HotelBooking.Application.Common.Pagination;
using HotelBooking.Application.Features.Deals.CreateDeal;
using HotelBooking.Application.Features.Deals.DeleteDeal;
using HotelBooking.Application.Features.Deals.GetDealById;
using HotelBooking.Application.Features.Deals.GetDeals;
using HotelBooking.Application.Features.Deals.UpdateDeal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.API.Controllers;

[ApiController]
[Route("api/deals")]
public class DealsController : ControllerBase
{
    private readonly CreateDealService _createDealService;
    private readonly GetDealsService _getDealsService;
    private readonly GetDealByIdService _getDealByIdService;
    private readonly UpdateDealService _updateDealService;
    private readonly DeleteDealService _deleteDealService;

    public DealsController(
        CreateDealService createDealService,
        GetDealsService getDealsService,
        GetDealByIdService getDealByIdService,
        UpdateDealService updateDealService,
        DeleteDealService deleteDealService)
    {
        _createDealService = createDealService;
        _getDealsService = getDealsService;
        _getDealByIdService = getDealByIdService;
        _updateDealService = updateDealService;
        _deleteDealService = deleteDealService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<GetDealsResponse>>> GetAll(
        [FromQuery] PaginationRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _getDealsService.GetAllAsync(request, cancellationToken);

        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<GetDealByIdResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _getDealByIdService.GetAsync(id, cancellationToken);

        if (!result.IsSuccess)
        {
            return NotFound(new
            {
                message = result.Error
            });
        }

        return Ok(result.Value);
    }

    [Authorize(Policy = AuthorizationPolicies.ManageDeals)]
    [HttpPost]
    public async Task<ActionResult<CreateDealResponse>> Create(CreateDealRequest request,
        CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdClaim, out var currentUserId))
        {
            return Unauthorized();
        }

        var result = await _createDealService.CreateAsync(
            request,
            currentUserId,
            User.IsInRole("Admin"),
            cancellationToken);

        if (!result.IsSuccess)
        {
            if (result.Error == "Hotel not found.")
            {
                return NotFound(new
                {
                    message = result.Error
                });
            }

            if (result.Error == "You are not allowed to manage deals for this hotel.")
            {
                return Forbid();
            }

            return Conflict(new
            {
                message = result.Error
            });
        }

        return StatusCode(StatusCodes.Status201Created, result.Value);
    }

    [Authorize(Policy = AuthorizationPolicies.ManageDeals)]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<UpdateDealResponse>> Update(
        int id,
        UpdateDealRequest request,
        CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdClaim, out var currentUserId))
        {
            return Unauthorized();
        }

        var result = await _updateDealService.UpdateAsync(
            id,
            request,
            currentUserId,
            User.IsInRole("Admin"),
            cancellationToken);

        if (!result.IsSuccess)
        {
            if (result.Error == "Deal not found." || result.Error == "Hotel not found.")
            {
                return NotFound(new
                {
                    message = result.Error
                });
            }

            if (result.Error == "You are not allowed to manage deals for this hotel.")
            {
                return Forbid();
            }

            return Conflict(new
            {
                message = result.Error
            });
        }

        return Ok(result.Value);
    }

    [Authorize(Policy = AuthorizationPolicies.ManageDeals)]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdClaim, out var currentUserId))
        {
            return Unauthorized();
        }

        var result = await _deleteDealService.DeleteAsync(
            id,
            currentUserId,
            User.IsInRole("Admin"),
            cancellationToken);

        if (!result.IsSuccess)
        {
            if (result.Error == "Deal not found." || result.Error == "Hotel not found.")
            {
                return NotFound(new
                {
                    message = result.Error
                });
            }

            if (result.Error == "You are not allowed to manage deals for this hotel.")
            {
                return Forbid();
            }

            return BadRequest(new
            {
                message = result.Error
            });
        }

        return NoContent();
    }
}