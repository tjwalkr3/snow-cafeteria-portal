using Microsoft.AspNetCore.Mvc;
using Cafeteria.Api.DTOs;

namespace Cafeteria.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StationController : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<StationDto>> GetStations()
    {
        throw new NotImplementedException();
    }

    [HttpGet("{id}")]
    public ActionResult<StationDto> GetStation(int id)
    {
        throw new NotImplementedException();
    }

    [HttpGet("location/{locationId}")]
    public ActionResult<IEnumerable<StationDto>> GetStationsByLocation(int locationId)
    {
        throw new NotImplementedException();
    }

    [HttpPost]
    public ActionResult<StationDto> CreateStation(StationDto stationDto)
    {
        throw new NotImplementedException();
    }

    [HttpPut("{id}")]
    public IActionResult UpdateStation(int id, StationDto stationDto)
    {
        throw new NotImplementedException();
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteStation(int id)
    {
        throw new NotImplementedException();
    }
}