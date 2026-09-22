using HotelBooking.API.Authorization;
using HotelBooking.Application.Common.Pagination;
using HotelBooking.Application.Features.Cities.CreateCity;
using HotelBooking.Application.Features.Cities.DeleteCity;
using HotelBooking.Application.Features.Cities.GetCities;
using HotelBooking.Application.Features.Cities.GetCityById;
using HotelBooking.Application.Features.Cities.UpdateCity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.API.Controllers;

[ApiController]
[Route("api/cities")]
public class CitiesController : ControllerBase
{
    private readonly CreateCityService _createCityService;
    private readonly GetCitiesService _getCitiesService;
    private readonly DeleteCityService _deleteCityService;
    private readonly UpdateCityService _updateCityService;
    private readonly GetCityByIdService _getCityByIdService;


    public CitiesController(
        CreateCityService createCityService,
        GetCitiesService getCitiesService,
        DeleteCityService deleteCityService,
        UpdateCityService updateCityService,
        GetCityByIdService getCityByIdService)
    {
        _createCityService = createCityService;
        _getCitiesService = getCitiesService;
        _deleteCityService = deleteCityService;
        _updateCityService = updateCityService;
        _getCityByIdService = getCityByIdService;
    }


    [Authorize(Policy = AuthorizationPolicies.ManageCities)]
    [HttpPost]
    public async Task<ActionResult<CreateCityResponse>> Create(
        CreateCityRequest request,
        CancellationToken cancellationToken)
    {
        var result =
            await _createCityService.CreateAsync(
                request,
                cancellationToken);


        if (!result.IsSuccess)
        {
            return Conflict(new
            {
                message = result.Error
            });
        }


        return CreatedAtAction(
            nameof(GetById),
            new
            {
                id = result.Value!.Id
            },
            result.Value);
    }


    [HttpGet]
    public async Task<ActionResult<PagedResult<GetCitiesResponse>>> GetAll(
        [FromQuery] PaginationRequest request,
        CancellationToken cancellationToken)
    {
        var cities =
            await _getCitiesService.GetAllAsync(
                request,
                cancellationToken);

        return Ok(cities);
    }


    [Authorize(Policy = AuthorizationPolicies.ManageCities)]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        var result =
            await _deleteCityService.DeleteAsync(
                id,
                cancellationToken);


        if (!result.IsSuccess)
        {
            if (result.Error == "City not found.")
            {
                return NotFound(new
                {
                    message = result.Error
                });
            }


            return BadRequest(new
            {
                message = result.Error
            });
        }


        return NoContent();
    }


    [Authorize(Policy = AuthorizationPolicies.ManageCities)]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<UpdateCityResponse>> Update(
        int id,
        UpdateCityRequest request,
        CancellationToken cancellationToken)
    {
        var result =
            await _updateCityService.UpdateAsync(
                id,
                request,
                cancellationToken);


        if (!result.IsSuccess)
        {
            if (result.Error == "City not found.")
            {
                return NotFound(new
                {
                    message = result.Error
                });
            }


            return Conflict(new
            {
                message = result.Error
            });
        }


        return Ok(result.Value);
    }


    [HttpGet("{id:int}")]
    public async Task<ActionResult<GetCityByIdResponse>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var result =
            await _getCityByIdService.GetAsync(
                id,
                cancellationToken);


        if (!result.IsSuccess)
        {
            return NotFound(new
            {
                message = result.Error
            });
        }


        return Ok(result.Value);
    }
}