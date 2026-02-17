using Microsoft.AspNetCore.Mvc;
using Cafeteria.Shared.DTOs.Order;
using Cafeteria.Api.Services.Orders;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Cafeteria.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class OrderController(IOrderService orderService) : ControllerBase
{
    private readonly IOrderService _orderService = orderService;

    [HttpGet("{id}")]
    public async Task<ActionResult<OrderDto>> GetOrderById(int id)
    {
        var result = await _orderService.GetOrderById(id);
        if (result == null)
            return NotFound();
        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<List<OrderDto>>> GetAllOrders()
    {
        var result = await _orderService.GetAllOrders();
        return Ok(result);
    }

    [HttpGet("with-customer")]
    public async Task<ActionResult<List<OrderWithCustomerDto>>> GetAllOrdersWithCustomer()
    {
        var result = await _orderService.GetAllOrdersWithCustomer();
        return Ok(result);
    }

    [HttpGet("with-customer/{id}")]
    public async Task<ActionResult<OrderWithCustomerDto>> GetOrderWithCustomerById(int id)
    {
        var result = await _orderService.GetOrderWithCustomerById(id);
        if (result == null)
            return NotFound();
        return Ok(result);
    }

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
    public async Task<ActionResult<OrderDto>> CreateOrder([FromBody] CreateOrderDto createOrderDto)
    {
        var email = User.FindFirst(ClaimTypes.Email)?.Value ?? User.FindFirst("preferred_username")?.Value;

        if (string.IsNullOrEmpty(email))
        {
            return BadRequest("Email not found in token claims");
        }

        var result = await _orderService.CreateOrder(createOrderDto, email);
        return CreatedAtAction(nameof(GetOrderById), new { id = result.Id }, result);
    }
}
