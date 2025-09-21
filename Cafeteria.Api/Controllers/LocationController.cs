using Microsoft.AspNetCore.Mvc;
using Cafeteria.Api.DTOs;

namespace Cafeteria.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LocationController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CafeteriaLocationDto>>> GetLocations()
    {
        throw new NotImplementedException();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CafeteriaLocationDto>> GetLocation(int id)
    {
        throw new NotImplementedException();
    }

    [HttpPost]
    public async Task<ActionResult<CafeteriaLocationDto>> CreateLocation(CafeteriaLocationDto locationDto)
    {
        throw new NotImplementedException();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateLocation(int id, CafeteriaLocationDto locationDto)
    {
        throw new NotImplementedException();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLocation(int id)
    {
        throw new NotImplementedException();
    }

    [HttpGet("{id}/business-hours")]
    public async Task<ActionResult<IEnumerable<LocationBusinessHoursDto>>> GetLocationBusinessHours(int id)
    {
        throw new NotImplementedException();
    }
}