using Microsoft.AspNetCore.Mvc;
using Cafeteria.Api.Authorization;
using Cafeteria.Api.Services.Stations;
using Cafeteria.Shared.DTOs.Menu;
using Cafeteria.Shared.Enums;
using Microsoft.AspNetCore.Authorization;

namespace Cafeteria.Api.Controllers;

[ApiController]
[Route("api/station")]
public class StationController : ControllerBase
{
    private readonly IStationService _stationService;

    public StationController(IStationService stationService)
    {
        _stationService = stationService;
    }

    [HttpGet]
    public async Task<List<StationDto>> GetAllStations()
    {
        return await _stationService.GetAllStations();
    }

    [HttpGet("location/{locationId:int}")]
    public async Task<List<StationDto>> GetStationsByLocationId(int locationId)
    {
        return await _stationService.GetStationsByLocationId(locationId);
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

    [HttpGet("{stationId:int}/hours")]
    public async Task<List<StationBusinessHoursDto>> GetStationBusinessHoursByStationId(int stationId)
    {
        return await _stationService.GetStationBusinessHoursByStationId(stationId);
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

    [Authorize]
    [RequireUserRole("admin", "food-service")]
    [HttpPost("location/{locationId:int}")]
    public async Task<IActionResult> CreateStationByLocationId(int locationId, [FromBody] StationUpsertRequest request)
    {
        await _stationService.CreateStationByLocationId(locationId, request.Name, request.Description, request.IconId);
        return NoContent();
    }

    [Authorize]
    [RequireUserRole("admin", "food-service")]
    [HttpPut("{stationId:int}")]
    public async Task<IActionResult> UpdateStationById(int stationId, [FromBody] StationUpsertRequest request)
    {
        await _stationService.UpdateStationById(stationId, request.Name, request.Description, request.IconId);
        return NoContent();
    }

    [Authorize]
    [RequireUserRole("admin", "food-service")]
    [HttpDelete("{stationId:int}")]
    public async Task<IActionResult> DeleteStationById(int stationId)
    {
        await _stationService.DeleteStationById(stationId);
        return NoContent();
    }

    [Authorize]
    [RequireUserRole("admin", "food-service")]
    [HttpPost("{stationId:int}/hours")]
    public async Task<IActionResult> AddStationHoursByStationId(int stationId, [FromBody] StationHoursRequest request)
    {
        if (!Enum.IsDefined(typeof(WeekDay), request.WeekdayId))
        {
            return BadRequest($"Invalid weekdayId {request.WeekdayId}");
        }

        var weekday = (WeekDay)request.WeekdayId;
        await _stationService.AddStationHoursByStationId(stationId, request.StartTime, request.EndTime, weekday);
        return NoContent();
    }

    [Authorize]
    [RequireUserRole("admin", "food-service")]
    [HttpPut("hours/{stationHrsId:int}")]
    public async Task<IActionResult> UpdateStationHoursById(int stationHrsId, [FromBody] StationHoursRequest request)
    {
        if (!Enum.IsDefined(typeof(WeekDay), request.WeekdayId))
        {
            return BadRequest($"Invalid weekdayId {request.WeekdayId}");
        }

        var weekday = (WeekDay)request.WeekdayId;
        await _stationService.UpdateStationHoursById(stationHrsId, request.StartTime, request.EndTime, weekday);
        return NoContent();
    }

    [Authorize]
    [RequireUserRole("admin", "food-service")]
    [HttpDelete("hours/{stationHrsId:int}")]
    public async Task<IActionResult> DeleteStationHoursById(int stationHrsId)
    {
        await _stationService.DeleteStationHoursById(stationHrsId);
        return NoContent();
    }
}

public record StationUpsertRequest(string Name, string? Description, int? IconId = null);

public record StationHoursRequest(DateTime StartTime, DateTime EndTime, int WeekdayId);
