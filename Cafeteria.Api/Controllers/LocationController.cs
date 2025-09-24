using Microsoft.AspNetCore.Mvc;
using Cafeteria.Shared.DTOs;

namespace Cafeteria.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LocationController : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<CafeteriaLocationDto>> GetLocations()
    {
        throw new NotImplementedException();
    }

    [HttpGet("{id}")]
    public ActionResult<CafeteriaLocationDto> GetLocation(int id)
    {
        throw new NotImplementedException();
    }

    [HttpPost]
    public ActionResult<CafeteriaLocationDto> CreateLocation(CafeteriaLocationDto locationDto)
    {
        throw new NotImplementedException();
    }

    [HttpPut("{id}")]
    public IActionResult UpdateLocation(int id, CafeteriaLocationDto locationDto)
    {
        throw new NotImplementedException();
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteLocation(int id)
    {
        throw new NotImplementedException();
    }

    [HttpGet("{id}/business-hours")]
    public ActionResult<IEnumerable<LocationBusinessHoursDto>> GetLocationBusinessHours(int id)
    {
        throw new NotImplementedException();
    }
}