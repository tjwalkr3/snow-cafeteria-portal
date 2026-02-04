using Microsoft.AspNetCore.Mvc;
using Cafeteria.Shared.DTOs.Swipe;
using Cafeteria.Api.Services.Swipes;
using Microsoft.AspNetCore.Authorization;

namespace Cafeteria.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SwipeController : ControllerBase
{
    private readonly ISwipeService _swipeService;

    public SwipeController(ISwipeService swipeService)
    {
        _swipeService = swipeService;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SwipeDto>> GetSwipesByUserID(int id)
    {
        try
        {
            var result = await _swipeService.GetSwipesByUserID(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound("User not found.");
        }
    }

    [HttpGet("email/{email}")]
    public async Task<ActionResult<SwipeDto>> GetSwipesByEmail(string email)
    {
        try
        {
            var result = await _swipeService.GetSwipesByEmail(email);
            if (result == null)
                return NotFound();
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound($"No swipes found for email {email}.");
        }
    }

    [HttpGet("all-customers")]
    public async Task<ActionResult<List<CustomerSwipeDto>>> GetAllCustomers()
    {
        try
        {
            var result = await _swipeService.GetAllCustomers();
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error retrieving customers: {ex.Message}");
        }
    }
}
