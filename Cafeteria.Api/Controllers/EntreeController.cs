using Microsoft.AspNetCore.Mvc;
using Cafeteria.Shared.DTOs.Menu;
using Cafeteria.Api.Authorization;
using Cafeteria.Api.Services.Entrees;
using Microsoft.AspNetCore.Authorization;

namespace Cafeteria.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EntreeController(IEntreeService entreeService) : ControllerBase
{
    private readonly IEntreeService _entreeService = entreeService;

    [HttpGet("{id}")]
    public async Task<ActionResult<EntreeDto>> GetEntreeById(int id)
    {
        var result = await _entreeService.GetEntreeById(id);
        if (result == null)
            return NotFound();
        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<List<EntreeDto>>> GetAllEntrees()
    {
        var result = await _entreeService.GetAllEntrees();
        return Ok(result);
    }

    [HttpGet("station/{stationId}")]
    public async Task<ActionResult<List<EntreeDto>>> GetEntreesByStationId(int stationId)
    {
        var result = await _entreeService.GetEntreesByStationId(stationId);
        return Ok(result);
    }

    [HttpGet("station/{stationId}/swipe")]
    public async Task<ActionResult<List<EntreeDto>>> GetSwipeEntreesByStationId(int stationId)
    {
        var result = await _entreeService.GetSwipeEntreesByStationId(stationId);
        return Ok(result);
    }

    [HttpGet("station/{stationId}/card")]
    public async Task<ActionResult<List<EntreeDto>>> GetCardEntreesByStationId(int stationId)
    {
        var result = await _entreeService.GetCardEntreesByStationId(stationId);
        return Ok(result);
    }

    [Authorize]
    [RequireUserRole("admin", "food-service")]
    [HttpPost]
    public async Task<ActionResult<EntreeDto>> CreateEntree([FromBody] EntreeDto entreeDto)
    {
        var result = await _entreeService.CreateEntree(entreeDto);
        return CreatedAtAction(nameof(GetEntreeById), new { id = result.Id }, result);
    }

    [Authorize]
    [RequireUserRole("admin", "food-service")]
    [HttpPut("{id}")]
    public async Task<ActionResult<EntreeDto>> UpdateEntreeById(int id, [FromBody] EntreeDto entreeDto)
    {
        var result = await _entreeService.UpdateEntreeById(id, entreeDto);
        if (result == null)
            return NotFound();
        return Ok(result);
    }

    [Authorize]
    [RequireUserRole("admin", "food-service")]
    [HttpPut("{id}/stock")]
    public async Task<IActionResult> SetStockStatusById(int id, [FromBody] bool inStock)
    {
        var result = await _entreeService.SetStockStatusById(id, inStock);
        if (!result)
            return NotFound();
        return NoContent();
    }

    [Authorize]
    [RequireUserRole("admin", "food-service")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEntreeById(int id)
    {
        var result = await _entreeService.DeleteEntreeById(id);
        if (!result)
            return NotFound();
        return NoContent();
    }
}
