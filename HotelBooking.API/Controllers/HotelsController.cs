using HotelBooking.API.Authorization;
using HotelBooking.Application.Features.Hotels.CreateHotel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.API.Controllers;

[ApiController]
[Route("api/hotels")]
public class HotelsController : ControllerBase
{
    private readonly CreateHotelService _createHotelService;


    public HotelsController(
        CreateHotelService createHotelService)
    {
        _createHotelService = createHotelService;
    }


    [Authorize(Policy = AuthorizationPolicies.ManageHotels)]
    [HttpPost]
    public async Task<ActionResult<CreateHotelResponse>> Create(
        CreateHotelRequest request,
        CancellationToken cancellationToken)
    {
        var result =
            await _createHotelService.CreateAsync(
                request,
                cancellationToken);


        if (!result.IsSuccess)
        {
            if (result.Error == "City not found."
                || result.Error == "Owner not found.")
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


        return CreatedAtAction(
            nameof(GetById),
            new
            {
                id = result.Value!.Id
            },
            result.Value);
    }


    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        // implement later
        return Ok();
    }
}