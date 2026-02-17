using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Cafeteria.Api.Services.Locations;
using Cafeteria.Shared.DTOs.Menu;
using Cafeteria.Shared.Enums;

namespace Cafeteria.Api.Controllers;

[ApiController]
[Route("api/location")]
public class LocationController(ILocationService locationService) : ControllerBase
{
    private readonly ILocationService _locationService = locationService;

    [HttpGet]
    public async Task<List<LocationDto>> GetAllLocations()
    {
        return await _locationService.GetAllLocations();
    }

    [HttpGet("{locationId:int}")]
    public async Task<ActionResult<LocationDto>> GetLocationById(int locationId)
    {
        LocationDto? location = await _locationService.GetLocationByID(locationId);

        if (location is null)
        {
            return NotFound();
        }

        return Ok(location);
    }

    [HttpGet("{locationId:int}/hours")]
    public async Task<List<LocationBusinessHoursDto>> GetLocationBusinessHoursByLocationId(int locationId)
    {
        return await _locationService.GetLocationBusinessHoursByLocationId(locationId);
    }

    [HttpGet("hours/{locationHrsId:int}")]
    public async Task<ActionResult<LocationBusinessHoursDto>> GetLocationBusinessHoursById(int locationHrsId)
    {
        LocationBusinessHoursDto? hours = await _locationService.GetLocationBusinessHoursById(locationHrsId);

        if (hours is null)
        {
            return NotFound();
        }

        return Ok(hours);
    }

    [Authorize]
    [HttpGet("authenticated")]
    public IActionResult GetAuthenticatedLocation()
    {
        return Ok(new { username = User.Identity?.Name ?? User.FindFirst("preferred_username")?.Value });
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateLocation([FromBody] LocationUpsertRequest request)
    {
        await _locationService.CreateLocation(request.Name, request.Description);
        return NoContent();
    }

    [Authorize]
    [HttpPut("{locationId:int}")]
    public async Task<IActionResult> UpdateLocationById(int locationId, [FromBody] LocationUpsertRequest request)
    {
        await _locationService.UpdateLocationById(locationId, request.Name, request.Description);
        return NoContent();
    }

    [Authorize]
    [HttpDelete("{locationId:int}")]
    public async Task<IActionResult> DeleteLocationById(int locationId)
    {
        await _locationService.DeleteLocationById(locationId);
        return NoContent();
    }

    [Authorize]
    [HttpPost("{locationId:int}/hours")]
    public async Task<IActionResult> AddLocationHoursByLocationId(int locationId, [FromBody] LocationHoursRequest request)
    {
        if (!Enum.IsDefined(typeof(WeekDay), request.WeekdayId))
        {
            return BadRequest($"Invalid weekdayId {request.WeekdayId}");
        }

        var weekday = (WeekDay)request.WeekdayId;
        await _locationService.AddLocationHoursByLocationId(locationId, request.StartTime, request.EndTime, weekday);
        return NoContent();
    }

    [Authorize]
    [HttpPut("hours/{locationHrsId:int}")]
    public async Task<IActionResult> UpdateLocationHoursById(int locationHrsId, [FromBody] LocationHoursRequest request)
    {
        if (!Enum.IsDefined(typeof(WeekDay), request.WeekdayId))
        {
            return BadRequest($"Invalid weekdayId {request.WeekdayId}");
        }

        var weekday = (WeekDay)request.WeekdayId;
        await _locationService.UpdateLocationHoursById(locationHrsId, request.StartTime, request.EndTime, weekday);
        return NoContent();
    }

    [Authorize]
    [HttpDelete("hours/{locationHrsId:int}")]
    public async Task<IActionResult> DeleteLocationHoursById(int locationHrsId)
    {
        await _locationService.DeleteLocationHoursById(locationHrsId);
        return NoContent();
    }
}

public record LocationUpsertRequest(string Name, string? Description);

public record LocationHoursRequest(DateTime StartTime, DateTime EndTime, int WeekdayId);
