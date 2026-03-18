using Cafeteria.Api.Services.FoodOptions;
using Cafeteria.Api.Authorization;
using Cafeteria.Shared.DTOs.Menu;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cafeteria.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FoodOptionController(IFoodOptionService foodOptionService) : ControllerBase
{
    private readonly IFoodOptionService _foodOptionService = foodOptionService;

    [HttpGet("{id}")]
    public async Task<ActionResult<FoodOptionDto>> GetFoodOptionById(int id)
    {
        var result = await _foodOptionService.GetFoodOptionById(id);
        if (result == null)
            return NotFound();
        return Ok(result);
    }

    [HttpGet]
    public async Task<List<FoodOptionDto>> GetAllFoodOptions()
    {
        return await _foodOptionService.GetAllFoodOptions();
    }

    [HttpGet("entree/{entreeId}")]
    public async Task<List<FoodOptionDto>> GetFoodOptionsByEntreeId(int entreeId)
    {
        return await _foodOptionService.GetFoodOptionsByEntreeId(entreeId);
    }

    [HttpGet("side/{sideId}")]
    public async Task<List<FoodOptionDto>> GetFoodOptionsBySideId(int sideId)
    {
        return await _foodOptionService.GetFoodOptionsBySideId(sideId);
    }

    [Authorize]
    [RequireUserRole("admin", "food-service")]
    [HttpPost]
    public async Task<FoodOptionDto> CreateFoodOption([FromBody] FoodOptionDto foodOptionDto)
    {
        return await _foodOptionService.CreateFoodOption(foodOptionDto);
    }

    [Authorize]
    [RequireUserRole("admin", "food-service")]
    [HttpPut("{id}")]
    public async Task<ActionResult<FoodOptionDto>> UpdateFoodOptionById(int id, [FromBody] FoodOptionDto foodOptionDto)
    {
        var result = await _foodOptionService.UpdateFoodOptionById(id, foodOptionDto);
        if (result == null)
            return NotFound();
        return Ok(result);
    }

    [Authorize]
    [RequireUserRole("admin", "food-service")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFoodOptionById(int id)
    {
        var result = await _foodOptionService.DeleteFoodOptionById(id);
        if (!result)
            return NotFound();
        return NoContent();
    }
}
