using Microsoft.AspNetCore.Mvc;
using Cafeteria.Shared.DTOs.Swipe;
using Cafeteria.Api.Services.Swipes;
using Microsoft.AspNetCore.Authorization;

namespace Cafeteria.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SwipeController(ISwipeService swipeService, ILogger<SwipeController> logger) : ControllerBase
{
    private readonly ISwipeService _swipeService = swipeService;
    private readonly ILogger<SwipeController> _logger = logger;

    [HttpGet("{id}")]
    public async Task<ActionResult<SwipeDto>> GetSwipesByUserID(int id)
    {
        try
        {
            _logger.LogInformation("Fetching swipes for user ID: {BadgerId}", id);
            var result = await _swipeService.GetSwipesByUserID(id);
            if (result == null)
            {
                _logger.LogWarning("No swipes found for user ID: {BadgerId}", id);
                return NotFound();
            }
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "User not found with ID: {BadgerId}", id);
            return NotFound("User not found.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error fetching swipes for user ID: {BadgerId}", id);
            return StatusCode(500, "An error occurred while retrieving swipes.");
        }
    }

    [HttpGet("email/{email}")]
    public async Task<ActionResult<SwipeDto>> GetSwipesByEmail(string email)
    {
        try
        {
            _logger.LogInformation("Fetching swipes for email: {Email}", email);
            var result = await _swipeService.GetSwipesByEmail(email);
            // Return OK with null result if no active swipes found (customer can pay with card)
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error fetching swipes for email: {Email}", email);
            return StatusCode(500, "An error occurred while retrieving swipes.");
        }
    }

    [HttpGet("all-customers")]
    public async Task<ActionResult<List<CustomerSwipeDto>>> GetAllCustomers()
    {
        try
        {
            _logger.LogInformation("Fetching all customers with swipe balances");
            var result = await _swipeService.GetAllCustomers();
            _logger.LogInformation("Successfully retrieved {Count} customers", result.Count);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all customers: {ErrorMessage}", ex.Message);
            return BadRequest($"Error retrieving customers. ");
        }
    }
}
