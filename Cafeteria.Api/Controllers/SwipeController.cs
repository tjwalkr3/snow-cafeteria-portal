using Microsoft.AspNetCore.Mvc;
using Cafeteria.Shared.DTOs.Swipe;
using Cafeteria.Api.Services.Swipes;

namespace Cafeteria.Api.Controllers;

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
}