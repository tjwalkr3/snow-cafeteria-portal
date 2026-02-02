using Microsoft.AspNetCore.Mvc;
using Cafeteria.Api.Services.Customer;
using Microsoft.AspNetCore.Authorization;

namespace Cafeteria.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CustomerController(ICustomerService customerService) : ControllerBase
{
    private readonly ICustomerService _customerService = customerService;

    [HttpPost("check")]
    public async Task<IActionResult> RegisterOrUpdate()
    {
        var email = User.FindFirst("email")?.Value ?? User.FindFirst("preferred_username")?.Value;
        var name = User.Identity?.Name ?? User.FindFirst("name")?.Value;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(name))
        {
            return BadRequest("Email or name not found in JWT token.");
        }

        await _customerService.EnsureCustomerExists(email, name);
        
        return Ok(new { message = "Customer registered or already exists." });
    }
}