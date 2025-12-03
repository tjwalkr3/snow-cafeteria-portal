using Microsoft.AspNetCore.Mvc;
using Cafeteria.Shared.DTOs;
using Cafeteria.Api.Services;

namespace Cafeteria.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SideController : ControllerBase
{
    private readonly ISideService _sideService;

    public SideController(ISideService sideService)
    {
        _sideService = sideService;
    }

    [HttpPost]
    public async Task<ActionResult<SideDto>> CreateSide([FromBody] SideDto sideDto)
    {
        var result = await _sideService.CreateSide(sideDto);
        return CreatedAtAction(nameof(GetSideByID), new { id = result.Id }, result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SideDto>> GetSideByID(int id)
    {
        var result = await _sideService.GetSideByID(id);
        if (result == null)
            return NotFound();
        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<List<SideDto>>> GetAllSides()
    {
        var result = await _sideService.GetAllSides();
        return Ok(result);
    }

    [HttpGet("station/{stationId}")]
    public async Task<ActionResult<List<SideDto>>> GetSidesByStationID(int stationId)
    {
        var result = await _sideService.GetSidesByStationID(stationId);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<SideDto>> UpdateSideByID(int id, [FromBody] SideDto sideDto)
    {
        var result = await _sideService.UpdateSideByID(id, sideDto);
        if (result == null)
            return NotFound();
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSideByID(int id)
    {
        var result = await _sideService.DeleteSideByID(id);
        if (!result)
            return NotFound();
        return NoContent();
    }
}
