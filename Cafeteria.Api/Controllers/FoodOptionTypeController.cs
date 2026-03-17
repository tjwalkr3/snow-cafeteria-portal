using Cafeteria.Api.Services.FoodOptionTypes;
using Cafeteria.Api.Authorization;
using Cafeteria.Shared.DTOs.Menu;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cafeteria.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FoodOptionTypeController(IFoodOptionTypeService foodTypeService) : ControllerBase
{
    private readonly IFoodOptionTypeService _foodTypeService = foodTypeService;

    [HttpGet("{id}")]
    public async Task<ActionResult<FoodOptionTypeDto>> GetFoodOptionTypeByID(int id)
    {
        var result = await _foodTypeService.GetFoodOptionTypeByID(id);
        if (result == null)
            return NotFound();
        return Ok(result);
    }

    [HttpGet]
    public async Task<List<FoodOptionTypeDto>> GetAllFoodOptionTypes()
    {
        return await _foodTypeService.GetAllFoodOptionTypes();
    }

    [HttpGet("entree/{entreeId}")]
    public async Task<List<FoodOptionTypeDto>> GetFoodOptionTypesByEntreeId(int entreeId)
    {
        return await _foodTypeService.GetFoodOptionTypesByEntreeId(entreeId);
    }

    [HttpGet("with-options/entree/{entreeId}")]
    public async Task<List<FoodOptionTypeWithOptionsDto>> GetFoodOptionTypesWithOptionsByEntreeId(int entreeId)
    {
        return await _foodTypeService.GetFoodOptionTypesWithOptionsByEntreeId(entreeId);
    }

    [Authorize]
    [RequireUserRole("admin", "food-service")]
    [HttpPost]
    public async Task<FoodOptionTypeDto> CreateFoodOptionType([FromBody] FoodOptionTypeDto foodTypeDto)
    {
        return await _foodTypeService.CreateFoodOptionType(foodTypeDto);
    }

    [Authorize]
    [RequireUserRole("admin", "food-service")]
    [HttpPut("{id}")]
    public async Task<ActionResult<FoodOptionTypeDto>> UpdateFoodOptionTypeById(int id, [FromBody] FoodOptionTypeDto foodTypeDto)
    {
        var result = await _foodTypeService.UpdateFoodOptionTypeById(id, foodTypeDto);
        if (result == null)
            return NotFound();
        return Ok(result);
    }

    [Authorize]
    [RequireUserRole("admin", "food-service")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFoodOptionTypeById(int id)
    {
        var result = await _foodTypeService.DeleteFoodOptionTypeById(id);
        if (!result)
            return NotFound();
        return NoContent();
    }
}
