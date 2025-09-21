using Microsoft.AspNetCore.Mvc;
using Cafeteria.Api.DTOs;

namespace Cafeteria.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StationController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<StationDto>>> GetStations()
    {
        throw new NotImplementedException();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<StationDto>> GetStation(int id)
    {
        throw new NotImplementedException();
    }

    [HttpGet("location/{locationId}")]
    public async Task<ActionResult<IEnumerable<StationDto>>> GetStationsByLocation(int locationId)
    {
        throw new NotImplementedException();
    }

    [HttpPost]
    public async Task<ActionResult<StationDto>> CreateStation(StationDto stationDto)
    {
        throw new NotImplementedException();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateStation(int id, StationDto stationDto)
    {
        throw new NotImplementedException();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteStation(int id)
    {
        throw new NotImplementedException();
    }
}