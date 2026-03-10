using Microsoft.AspNetCore.Mvc;
using Cafeteria.Shared.DTOs.Menu;
using Cafeteria.Api.Services.Sides;
using Microsoft.AspNetCore.Authorization;

namespace Cafeteria.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SideController(ISideService sideService) : ControllerBase
{
    private readonly ISideService _sideService = sideService;

    [HttpGet("{id}")]
    public async Task<ActionResult<SideDto>> GetSideById(int id)
    {
        var result = await _sideService.GetSideById(id);
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
    public async Task<ActionResult<List<SideDto>>> GetSidesByStationId(int stationId)
    {
        var result = await _sideService.GetSidesByStationId(stationId);
        return Ok(result);
    }

    [HttpGet("station/{stationId}/with-options")]
    public async Task<ActionResult<List<SideWithOptionsDto>>> GetSidesByStationIdWithOptions(int stationId)
    {
        var result = await _sideService.GetSidesByStationIdWithOptions(stationId);
        return Ok(result);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<SideDto>> CreateSide([FromBody] SideDto sideDto)
    {
        var result = await _sideService.CreateSide(sideDto);
        return CreatedAtAction(nameof(GetSideById), new { id = result.Id }, result);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<ActionResult<SideDto>> UpdateSideById(int id, [FromBody] SideDto sideDto)
    {
        var result = await _sideService.UpdateSideById(id, sideDto);
        if (result == null)
            return NotFound();
        return Ok(result);
    }

    [Authorize]
    [HttpPut("{id}/stock")]
    public async Task<IActionResult> SetStockStatusById(int id, [FromBody] bool inStock)
    {
        var result = await _sideService.SetStockStatusById(id, inStock);
        if (!result)
            return NotFound();
        return NoContent();
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSideById(int id)
    {
        var result = await _sideService.DeleteSideById(id);
        if (!result)
            return NotFound();
        return NoContent();
    }
}
