using Microsoft.AspNetCore.Mvc;
using Cafeteria.Api.Services.Customer;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Cafeteria.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CustomerController(ICustomerService customerService, ILogger<CustomerController> logger) : ControllerBase
{
    private readonly ICustomerService _customerService = customerService;
    private readonly ILogger<CustomerController> _logger = logger;

    [HttpPost("check")]
    public async Task<IActionResult> RegisterOrUpdate()
    {
        var email = User.FindFirst(ClaimTypes.Email)?.Value ?? User.FindFirst("preferred_username")?.Value;
        var name = User.FindFirst("name")?.Value ?? User.FindFirst("preferred_username")?.Value;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(name))
        {
            return BadRequest("Email or name not found in JWT token.");
        }

        await _customerService.EnsureCustomerExists(email, name);

        return Ok(new { message = "Customer registered or already exists." });
    }
}