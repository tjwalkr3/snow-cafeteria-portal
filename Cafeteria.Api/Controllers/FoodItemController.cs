using Microsoft.AspNetCore.Mvc;
using Cafeteria.Shared.DTOs;

namespace Cafeteria.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FoodItemController : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<FoodItemDto>> GetFoodItems()
    {
        throw new NotImplementedException();
    }

    [HttpGet("{id}")]
    public ActionResult<FoodItemDto> GetFoodItem(int id)
    {
        throw new NotImplementedException();
    }

    [HttpGet("station/{stationId}")]
    public ActionResult<IEnumerable<FoodItemDto>> GetFoodItemsByStation(int stationId)
    {
        throw new NotImplementedException();
    }

    [HttpPost]
    public ActionResult<FoodItemDto> CreateFoodItem(FoodItemDto foodItemDto)
    {
        throw new NotImplementedException();
    }

    [HttpPut("{id}")]
    public IActionResult UpdateFoodItem(int id, FoodItemDto foodItemDto)
    {
        throw new NotImplementedException();
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteFoodItem(int id)
    {
        throw new NotImplementedException();
    }
}