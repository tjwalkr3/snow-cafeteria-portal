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
    public async Task<IActionResult> CreateLocation([FromBody] LocationDto location)
    {
        await _locationService.CreateLocation(location);
        return NoContent();
    }

    [HttpPut("{locationId:int}")]
    public async Task<IActionResult> UpdateLocation(int locationId, [FromBody] LocationDto location)
    {
        await _locationService.UpdateLocationByID(locationId, location);
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
    public async Task<IActionResult> AddLocationHours(int locationId, [FromBody] LocationBusinessHoursDto hours)
    {
        if (!Enum.IsDefined(typeof(WeekDay), hours.WeekdayId))
        {
            return BadRequest($"Invalid weekdayId {hours.WeekdayId}");
        }

        var weekday = (WeekDay)hours.WeekdayId;
        await _locationService.AddLocationHours(locationId, hours);
        return NoContent();
    }

    [HttpPut("hours/{locationHrsId:int}")]
    public async Task<IActionResult> UpdateLocationHours(int locationHrsId, [FromBody] LocationBusinessHoursDto hours)
    {
        if (!Enum.IsDefined(typeof(WeekDay), hours.WeekdayId))
        {
            return BadRequest($"Invalid weekdayId {hours.WeekdayId}");
        }

        var weekday = (WeekDay)hours.WeekdayId;
        await _locationService.UpdateLocationHoursById(locationHrsId, hours);
        return NoContent();
    }

    [HttpDelete("hours/{locationHrsId:int}")]
    public async Task<IActionResult> DeleteLocationHours(int locationHrsId)
    {
        await _locationService.DeleteLocationHrsById(locationHrsId);
        return NoContent();
    }
}