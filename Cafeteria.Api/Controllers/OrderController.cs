using Microsoft.AspNetCore.Mvc;
using Cafeteria.Shared.DTOs.Order;
using Cafeteria.Api.Authorization;
using Cafeteria.Api.Services.Orders;
using Cafeteria.Api.Services.Customer;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Cafeteria.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class OrderController(
    IOrderService orderService,
    ICreateOrderService createOrderService,
    IUserRoleService userRoleService
) : ControllerBase
{
    private readonly IOrderService _orderService = orderService;
    private readonly ICreateOrderService _createOrderService = createOrderService;
    private readonly IUserRoleService _userRoleService = userRoleService;

    [HttpGet("{id}")]
    public async Task<ActionResult<OrderDto>> GetOrderById(int id)
    {
        var summary = await _orderService.GetOrderWithCustomerById(id);
        if (summary == null)
        {
            return NotFound();
        }

        if (!await CanAccessOrderAsync(summary.CustomerEmail))
        {
            return Forbid();
        }

        var result = await _orderService.GetOrderById(id);
        if (result == null)
            return NotFound();
        return Ok(result);
    }

    [HttpGet]
    [RequireUserRole("admin", "food-service")]
    public async Task<ActionResult<List<OrderDto>>> GetAllOrders()
    {
        var result = await _orderService.GetAllOrders();
        return Ok(result);
    }

    [HttpGet("with-customer")]
    [RequireUserRole("admin", "food-service")]
    public async Task<ActionResult<List<OrderWithCustomerDto>>> GetAllOrdersWithCustomer()
    {
        var result = await _orderService.GetAllOrdersWithCustomer();
        return Ok(result);
    }

    [HttpGet("with-customer/{id}")]
    [RequireUserRole("admin", "food-service")]
    public async Task<ActionResult<OrderWithCustomerDto>> GetOrderWithCustomerById(int id)
    {
        var result = await _orderService.GetOrderWithCustomerById(id);
        if (result == null)
            return NotFound();
        return Ok(result);
    }

    [RequireUserRole("admin", "food-service")]
    [HttpGet("customer/{badgerId}")]
    public async Task<ActionResult<List<OrderWithCustomerDto>>> GetOrdersByCustomer(int badgerId)
    {
        var result = await _orderService.GetOrdersByCustomer(badgerId);
        return Ok(result);
    }

    [HttpGet("customer-email")]
    public async Task<ActionResult<List<OrderDto>>> GetOrdersByCustomerEmail()
    {
        var email = User.FindFirst(ClaimTypes.Email)?.Value ?? User.FindFirst("preferred_username")?.Value;

        if (string.IsNullOrEmpty(email))
        {
            return BadRequest("Email not found in token claims");
        }

        var result = await _orderService.GetOrdersByCustomerEmail(email);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<OrderDto>> CreateOrder([FromBody] BrowserOrder browserOrder)
    {
        var email = User.FindFirst(ClaimTypes.Email)?.Value ?? User.FindFirst("preferred_username")?.Value;

        if (string.IsNullOrEmpty(email))
        {
            var claimNames = string.Join(", ", User.Claims.Select(c => $"{c.Type}={c.Value}"));
            return BadRequest($"Email not found in token claims. Available: [{claimNames}]");
        }

        var result = await _createOrderService.CreateOrder(browserOrder, email);
        return CreatedAtAction(nameof(GetOrderById), new { id = result.Id }, result);
    }

    private async Task<bool> CanAccessOrderAsync(string? orderCustomerEmail)
    {
        var hasPrivilegedRole = await _userRoleService.UserHasAnyRoleAsync(User, "admin", "food-service");
        if (hasPrivilegedRole)
        {
            return true;
        }

        var requesterEmail = User.FindFirst(ClaimTypes.Email)?.Value ?? User.FindFirst("preferred_username")?.Value;
        if (string.IsNullOrWhiteSpace(requesterEmail) || string.IsNullOrWhiteSpace(orderCustomerEmail))
        {
            return false;
        }

        return string.Equals(orderCustomerEmail, requesterEmail, StringComparison.OrdinalIgnoreCase);
    }
}
