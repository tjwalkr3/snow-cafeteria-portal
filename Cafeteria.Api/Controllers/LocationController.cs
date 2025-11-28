using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cafeteria.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LocationController : ControllerBase
{
    [HttpGet("authenticated")]
    [Authorize]
    public IActionResult GetAuthenticatedLocation()
    {
        return Ok(new { username = "Test User" });
    }
}
