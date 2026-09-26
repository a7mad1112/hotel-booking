using HotelBooking.API.Authorization;
using HotelBooking.API.Extensions;
using HotelBooking.Application.Common.Pagination;
using HotelBooking.Application.Features.Amenities.CreateAmenity;
using HotelBooking.Application.Features.Amenities.DeleteAmenity;
using HotelBooking.Application.Features.Amenities.GetAmenities;
using HotelBooking.Application.Features.Amenities.GetAmenityById;
using HotelBooking.Application.Features.Amenities.UpdateAmenity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.API.Controllers;

[ApiController]
[Route("api/amenities")]
public class AmenitiesController : ControllerBase
{
    private readonly GetAmenitiesService _getAmenitiesService;
    private readonly GetAmenityByIdService _getAmenityByIdService;
    private readonly CreateAmenityService _createAmenityService;
    private readonly UpdateAmenityService _updateAmenityService;
    private readonly DeleteAmenityService _deleteAmenityService;

    public AmenitiesController(
        GetAmenitiesService getAmenitiesService,
        GetAmenityByIdService getAmenityByIdService,
        CreateAmenityService createAmenityService,
        UpdateAmenityService updateAmenityService,
        DeleteAmenityService deleteAmenityService)
    {
        _getAmenitiesService = getAmenitiesService;
        _getAmenityByIdService = getAmenityByIdService;
        _createAmenityService = createAmenityService;
        _updateAmenityService = updateAmenityService;
        _deleteAmenityService = deleteAmenityService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<GetAmenitiesResponse>>> GetAll(
        [FromQuery] PaginationRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _getAmenitiesService.GetAllAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<GetAmenityByIdResponse>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var result = await _getAmenityByIdService.GetAsync(id, cancellationToken);

        if (!result.IsSuccess)
        {
            return result.ToErrorResult();
        }

        return Ok(result.Value);
    }

    [Authorize(Policy = AuthorizationPolicies.ManageAmenities)]
    [HttpPost]
    public async Task<ActionResult<CreateAmenityResponse>> Create(
        CreateAmenityRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _createAmenityService.CreateAsync(request, cancellationToken);

        if (!result.IsSuccess)
        {
            return result.ToErrorResult();
        }

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Value!.Id },
            result.Value);
    }

    [Authorize(Policy = AuthorizationPolicies.ManageAmenities)]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<UpdateAmenityResponse>> Update(
        int id,
        UpdateAmenityRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _updateAmenityService.UpdateAsync(id, request, cancellationToken);

        if (!result.IsSuccess)
        {
            return result.ToErrorResult();
        }

        return Ok(result.Value);
    }

    [Authorize(Policy = AuthorizationPolicies.ManageAmenities)]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        var result = await _deleteAmenityService.DeleteAsync(id, cancellationToken);

        if (!result.IsSuccess)
        {
            return result.ToErrorResult();
        }

        return NoContent();
    }
}
