using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Cafeteria.Api.Services;
using Cafeteria.Shared.DTOs;
using Cafeteria.Shared.Enums;

namespace Cafeteria.Api.Controllers;

[ApiController]
[Route("api/location")]
public class LocationController : ControllerBase
{
    private readonly ILocationService _locationService;

    public LocationController(ILocationService locationService)
    {
        _locationService = locationService;
    }

    [HttpGet("authenticated")]
    [Authorize]
    public IActionResult GetAuthenticatedLocation()
    {
        return Ok(new { username = User.Identity?.Name ?? User.FindFirst("preferred_username")?.Value });
    }

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

    [HttpPost]
    public async Task<IActionResult> CreateLocation([FromBody] LocationUpsertRequest request)
    {
        await _locationService.CreateLocation(request.Name, request.Description);
        return NoContent();
    }

    [HttpPut("{locationId:int}")]
    public async Task<IActionResult> UpdateLocation(int locationId, [FromBody] LocationUpsertRequest request)
    {
        await _locationService.UpdateLocationByID(locationId, request.Name, request.Description);
        return NoContent();
    }

    [HttpDelete("{locationId:int}")]
    public async Task<IActionResult> DeleteLocation(int locationId)
    {
        await _locationService.DeleteLocationByID(locationId);
        return NoContent();
    }

    [HttpGet("{locationId:int}/hours")]
    public async Task<List<LocationBusinessHoursDto>> GetLocationBusinessHours(int locationId)
    {
        return await _locationService.GetLocationBusinessHours(locationId);
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

    [HttpPost("{locationId:int}/hours")]
    public async Task<IActionResult> AddLocationHours(int locationId, [FromBody] LocationHoursRequest request)
    {
        if (!Enum.IsDefined(typeof(WeekDay), request.WeekdayId))
        {
            return BadRequest($"Invalid weekdayId {request.WeekdayId}");
        }

        var weekday = (WeekDay)request.WeekdayId;
        await _locationService.AddLocationHours(locationId, request.StartTime, request.EndTime, weekday);
        return NoContent();
    }

    [HttpPut("hours/{locationHrsId:int}")]
    public async Task<IActionResult> UpdateLocationHours(int locationHrsId, [FromBody] LocationHoursRequest request)
    {
        if (!Enum.IsDefined(typeof(WeekDay), request.WeekdayId))
        {
            return BadRequest($"Invalid weekdayId {request.WeekdayId}");
        }

        var weekday = (WeekDay)request.WeekdayId;
        await _locationService.UpdateLocationHoursById(locationHrsId, request.StartTime, request.EndTime, weekday);
        return NoContent();
    }

    [HttpDelete("hours/{locationHrsId:int}")]
    public async Task<IActionResult> DeleteLocationHours(int locationHrsId)
    {
        await _locationService.DeleteLocationHrsById(locationHrsId);
        return NoContent();
    }
}

public record LocationUpsertRequest(string Name, string? Description);

public record LocationHoursRequest(DateTime StartTime, DateTime EndTime, int WeekdayId);
