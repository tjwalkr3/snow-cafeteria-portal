using Microsoft.AspNetCore.Mvc;
using Cafeteria.Shared.DTOs;
using Cafeteria.Api.Services;

namespace Cafeteria.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EntreeController : ControllerBase
{
    private readonly IEntreeService _entreeService;

    public EntreeController(IEntreeService entreeService)
    {
        _entreeService = entreeService;
    }

    [HttpPost]
    public async Task<ActionResult<EntreeDto>> CreateEntree([FromBody] EntreeDto entreeDto)
    {
        var result = await _entreeService.CreateEntree(entreeDto);
        return CreatedAtAction(nameof(GetEntreeByID), new { id = result.Id }, result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<EntreeDto>> GetEntreeByID(int id)
    {
        var result = await _entreeService.GetEntreeByID(id);
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
    public async Task<ActionResult<List<EntreeDto>>> GetEntreesByStationID(int stationId)
    {
        var result = await _entreeService.GetEntreesByStationID(stationId);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<EntreeDto>> UpdateEntreeByID(int id, [FromBody] EntreeDto entreeDto)
    {
        var result = await _entreeService.UpdateEntreeByID(id, entreeDto);
        if (result == null)
            return NotFound();
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEntreeByID(int id)
    {
        var result = await _entreeService.DeleteEntreeByID(id);
        if (!result)
            return NotFound();
        return NoContent();
    }
}
