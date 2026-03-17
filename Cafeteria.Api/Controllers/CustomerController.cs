using Microsoft.AspNetCore.Mvc;
using Cafeteria.Api.Services.Customer;
using Cafeteria.Shared.DTOs.Customer;
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

    [HttpGet("role")]
    public async Task<ActionResult<UserRoleDto>> GetCurrentUserRole([FromServices] IUserRoleService userRoleService)
    {
        if (User.Identity?.IsAuthenticated != true)
        {
            return Unauthorized();
        }

        var role = await userRoleService.GetUserRoleAsync(User);
        if (string.IsNullOrWhiteSpace(role))
        {
            return Unauthorized();
        }

        return Ok(new UserRoleDto { UserRole = role });
    }

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