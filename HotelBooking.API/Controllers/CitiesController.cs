using FluentValidation;
using HotelBooking.API.Authorization;
using HotelBooking.API.Extensions;
using HotelBooking.Application.Features.Cities.CreateCity;
using HotelBooking.Application.Features.Cities.DeleteCity;
using HotelBooking.Application.Features.Cities.GetCities;
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

    private readonly IValidator<CreateCityRequest> _createCityRequestValidator;
    private readonly IValidator<UpdateCityRequest> _updateCityRequestValidator;


    public CitiesController(
        CreateCityService createCityService,
        GetCitiesService getCitiesService,
        DeleteCityService deleteCityService,
        UpdateCityService updateCityService,
        IValidator<CreateCityRequest> createCityRequestValidator,
        IValidator<UpdateCityRequest> updateCityRequestValidator)
    {
        _createCityService = createCityService;
        _getCitiesService = getCitiesService;
        _deleteCityService = deleteCityService;
        _updateCityService = updateCityService;

        _createCityRequestValidator = createCityRequestValidator;
        _updateCityRequestValidator = updateCityRequestValidator;
    }


    [Authorize(Policy = AuthorizationPolicies.ManageCities)]
    [HttpPost]
    public async Task<ActionResult<CreateCityResponse>> Create(
        CreateCityRequest request,
        CancellationToken cancellationToken)
    {
        var validation =
            await _createCityRequestValidator.ValidateAsync(
                request,
                cancellationToken);


        if (!validation.IsValid)
        {
            return this.ValidationProblem(validation);
        }


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
            nameof(Create),
            new
            {
                id = result.Value!.Id
            },
            result.Value);
    }


    [HttpGet]
    public async Task<ActionResult<List<GetCitiesResponse>>> GetAll(
        CancellationToken cancellationToken)
    {
        var cities =
            await _getCitiesService.GetAllAsync(
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
        var validation =
            await _updateCityRequestValidator.ValidateAsync(
                request,
                cancellationToken);


        if (!validation.IsValid)
        {
            return this.ValidationProblem(validation);
        }


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
}