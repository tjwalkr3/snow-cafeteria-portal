using Microsoft.AspNetCore.Mvc;
using Cafeteria.Api.DTOs;

namespace Cafeteria.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FoodItemController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<FoodItemDto>>> GetFoodItems()
    {
        throw new NotImplementedException();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<FoodItemDto>> GetFoodItem(int id)
    {
        throw new NotImplementedException();
    }

    [HttpGet("station/{stationId}")]
    public async Task<ActionResult<IEnumerable<FoodItemDto>>> GetFoodItemsByStation(int stationId)
    {
        throw new NotImplementedException();
    }

    [HttpPost]
    public async Task<ActionResult<FoodItemDto>> CreateFoodItem(FoodItemDto foodItemDto)
    {
        throw new NotImplementedException();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateFoodItem(int id, FoodItemDto foodItemDto)
    {
        throw new NotImplementedException();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFoodItem(int id)
    {
        throw new NotImplementedException();
    }
}