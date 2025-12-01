using Microsoft.AspNetCore.Mvc;
using Cafeteria.Api.Services;
using Cafeteria.Shared.DTOs;
using Cafeteria.Shared.Enums;

namespace Cafeteria.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StationController : ControllerBase
{
    private readonly IStationService _stationService;

    public StationController(IStationService stationService)
    {
        _stationService = stationService;
    }

    [HttpGet("location/{locationId:int}")]
    public async Task<List<StationDto>> GetStationsByLocation(int locationId)
    {
        return await _stationService.GetStationsByLocation(locationId);
    }

    [HttpGet("{stationId:int}")]
    public async Task<ActionResult<StationDto>> GetStationById(int stationId)
    {
        StationDto? station = await _stationService.GetStationByID(stationId);

        if (station is null)
        {
            return NotFound();
        }

        return Ok(station);
    }

    [HttpPost("location/{locationId:int}")]
    public async Task<IActionResult> CreateStationForLocation(int locationId, [FromBody] StationUpsertRequest request)
    {
        await _stationService.CreateStationForLocation(locationId, request.Name, request.Description);
        return NoContent();
    }

    [HttpPut("{stationId:int}")]
    public async Task<IActionResult> UpdateStation(int stationId, [FromBody] StationUpsertRequest request)
    {
        await _stationService.UpdateStationByID(stationId, request.Name, request.Description);
        return NoContent();
    }

    [HttpDelete("{stationId:int}")]
    public async Task<IActionResult> DeleteStation(int stationId)
    {
        await _stationService.DeleteStationByID(stationId);
        return NoContent();
    }

    [HttpGet("{stationId:int}/hours")]
    public async Task<List<StationBusinessHoursDto>> GetStationBusinessHours(int stationId)
    {
        return await _stationService.GetStationBusinessHours(stationId);
    }

    [HttpGet("hours/{stationHrsId:int}")]
    public async Task<ActionResult<StationBusinessHoursDto>> GetStationBusinessHoursById(int stationHrsId)
    {
        StationBusinessHoursDto? stationHours = await _stationService.GetStationBusinessHoursById(stationHrsId);

        if (stationHours is null)
        {
            return NotFound();
        }

        return Ok(stationHours);
    }

    [HttpPost("{stationId:int}/hours")]
    public async Task<IActionResult> AddStationHours(int stationId, [FromBody] StationHoursRequest request)
    {
        if (!Enum.IsDefined(typeof(WeekDay), request.WeekdayId))
        {
            return BadRequest($"Invalid weekdayId {request.WeekdayId}");
        }

        var weekday = (WeekDay)request.WeekdayId;
        await _stationService.AddStationHours(stationId, request.StartTime, request.EndTime, weekday);
        return NoContent();
    }

    [HttpPut("hours/{stationHrsId:int}")]
    public async Task<IActionResult> UpdateStationHours(int stationHrsId, [FromBody] StationHoursRequest request)
    {
        if (!Enum.IsDefined(typeof(WeekDay), request.WeekdayId))
        {
            return BadRequest($"Invalid weekdayId {request.WeekdayId}");
        }

        var weekday = (WeekDay)request.WeekdayId;
        await _stationService.UpdateStationHoursById(stationHrsId, request.StartTime, request.EndTime, weekday);
        return NoContent();
    }

    [HttpDelete("hours/{stationHrsId:int}")]
    public async Task<IActionResult> DeleteStationHours(int stationHrsId)
    {
        await _stationService.DeleteStationHrsById(stationHrsId);
        return NoContent();
    }
}

public record StationUpsertRequest(string Name, string? Description);

public record StationHoursRequest(DateTime StartTime, DateTime EndTime, int WeekdayId);
