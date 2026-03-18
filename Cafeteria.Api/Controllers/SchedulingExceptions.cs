using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Cafeteria.Api.Authorization;
using Cafeteria.Api.Services.SchedulingExceptions;
using Cafeteria.Shared.DTOs.Menu;

namespace Cafeteria.Api.Controllers;

[ApiController]
[Route("api/SchedulingExceptions")]
public class SchedulingExceptionsController : ControllerBase
{
    private readonly ISchedulingExceptionsService _schedulingExceptionsService;

    public SchedulingExceptionsController(ISchedulingExceptionsService schedulingExceptionsService)
    {
        _schedulingExceptionsService = schedulingExceptionsService;
    }

    // Location Exception Endpoints

    [HttpGet("location/{locationId:int}")]
    public async Task<List<LocationExceptionHoursDto>> GetLocationExceptionsByLocationId(int locationId)
    {
        return await _schedulingExceptionsService.GetLocationExceptionsByLocationId(locationId);
    }

    [HttpGet("location/{exceptionId:int}/detail")]
    public async Task<ActionResult<LocationExceptionHoursDto>> GetLocationExceptionById(int exceptionId)
    {
        LocationExceptionHoursDto? exception = await _schedulingExceptionsService.GetLocationExceptionById(exceptionId);

        if (exception is null)
        {
            return NotFound();
        }

        return Ok(exception);
    }

    [Authorize]
    [RequireUserRole("admin", "food-service")]
    [HttpPost("location/{locationId:int}")]
    public async Task<IActionResult> AddLocationException(int locationId, [FromBody] SchedulingExceptionRequest request)
    {
        if (request.StartDateTime >= request.EndDateTime)
        {
            return BadRequest("Start time must be before end time.");
        }

        await _schedulingExceptionsService.AddLocationException(locationId, request.StartDateTime, request.EndDateTime, request.Reason);
        return NoContent();
    }

    [Authorize]
    [RequireUserRole("admin", "food-service")]
    [HttpPut("location/{exceptionId:int}")]
    public async Task<IActionResult> UpdateLocationException(int exceptionId, [FromBody] SchedulingExceptionRequest request)
    {
        if (request.StartDateTime >= request.EndDateTime)
        {
            return BadRequest("Start time must be before end time.");
        }

        LocationExceptionHoursDto? exception = await _schedulingExceptionsService.GetLocationExceptionById(exceptionId);
        if (exception is null)
        {
            return NotFound();
        }

        await _schedulingExceptionsService.UpdateLocationException(exceptionId, request.StartDateTime, request.EndDateTime, request.Reason);
        return NoContent();
    }

    [Authorize]
    [RequireUserRole("admin", "food-service")]
    [HttpDelete("location/{exceptionId:int}")]
    public async Task<IActionResult> DeleteLocationException(int exceptionId)
    {
        LocationExceptionHoursDto? exception = await _schedulingExceptionsService.GetLocationExceptionById(exceptionId);
        if (exception is null)
        {
            return NotFound();
        }

        await _schedulingExceptionsService.DeleteLocationException(exceptionId);
        return NoContent();
    }

    //Station Exception Endpoints

    [HttpGet("station/{stationId:int}")]
    public async Task<List<StationExceptionHoursDto>> GetStationExceptionsByStationId(int stationId)
    {
        return await _schedulingExceptionsService.GetStationExceptionsByStationId(stationId);
    }

    [HttpGet("station/{exceptionId:int}/detail")]
    public async Task<ActionResult<StationExceptionHoursDto>> GetStationExceptionById(int exceptionId)
    {
        StationExceptionHoursDto? exception = await _schedulingExceptionsService.GetStationExceptionById(exceptionId);

        if (exception is null)
        {
            return NotFound();
        }

        return Ok(exception);
    }

    [Authorize]
    [RequireUserRole("admin", "food-service")]
    [HttpPost("station/{stationId:int}")]
    public async Task<IActionResult> AddStationException(int stationId, [FromBody] SchedulingExceptionRequest request)
    {
        if (request.StartDateTime >= request.EndDateTime)
        {
            return BadRequest("Start time must be before end time.");
        }

        await _schedulingExceptionsService.AddStationException(stationId, request.StartDateTime, request.EndDateTime, request.Reason);
        return NoContent();
    }

    [Authorize]
    [RequireUserRole("admin", "food-service")]
    [HttpPut("station/{exceptionId:int}")]
    public async Task<IActionResult> UpdateStationException(int exceptionId, [FromBody] SchedulingExceptionRequest request)
    {
        if (request.StartDateTime >= request.EndDateTime)
        {
            return BadRequest("Start time must be before end time.");
        }

        StationExceptionHoursDto? exception = await _schedulingExceptionsService.GetStationExceptionById(exceptionId);
        if (exception is null)
        {
            return NotFound();
        }

        await _schedulingExceptionsService.UpdateStationException(exceptionId, request.StartDateTime, request.EndDateTime, request.Reason);
        return NoContent();
    }

    [Authorize]
    [RequireUserRole("admin", "food-service")]
    [HttpDelete("station/{exceptionId:int}")]
    public async Task<IActionResult> DeleteStationException(int exceptionId)
    {
        StationExceptionHoursDto? exception = await _schedulingExceptionsService.GetStationExceptionById(exceptionId);
        if (exception is null)
        {
            return NotFound();
        }

        await _schedulingExceptionsService.DeleteStationException(exceptionId);
        return NoContent();
    }
}

public record SchedulingExceptionRequest(DateTime StartDateTime, DateTime EndDateTime, string? Reason = null);
