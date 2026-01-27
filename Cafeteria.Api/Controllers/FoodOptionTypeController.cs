using Cafeteria.Api.Services.FoodOptionTypes;
using Cafeteria.Shared.DTOs.Menu;
using Microsoft.AspNetCore.Mvc;

namespace Cafeteria.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FoodOptionTypeController : ControllerBase
{
    private readonly IFoodOptionTypeService _foodTypeService;

    public FoodOptionTypeController(IFoodOptionTypeService foodTypeService)
    {
        _foodTypeService = foodTypeService;
    }

    [HttpPost]
    public async Task<FoodOptionTypeDto> CreateFoodType([FromBody] FoodOptionTypeDto foodTypeDto)
    {
        return await _foodTypeService.CreateFoodType(foodTypeDto);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<FoodOptionTypeDto>> GetFoodTypeByID(int id)
    {
        var result = await _foodTypeService.GetFoodTypeByID(id);
        if (result == null)
            return NotFound();
        return Ok(result);
    }

    [HttpGet]
    public async Task<List<FoodOptionTypeDto>> GetAllFoodTypes()
    {
        return await _foodTypeService.GetAllFoodTypes();
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<FoodOptionTypeDto>> UpdateFoodType(
        int id,
        [FromBody] FoodOptionTypeDto foodTypeDto
    )
    {
        var result = await _foodTypeService.UpdateFoodType(id, foodTypeDto);
        if (result == null)
            return NotFound();
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFoodType(int id)
    {
        var result = await _foodTypeService.DeleteFoodType(id);
        if (!result)
            return NotFound();
        return NoContent();
    }
}
