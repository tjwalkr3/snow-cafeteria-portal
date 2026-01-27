using Cafeteria.Api.Services.FoodOptions;
using Cafeteria.Shared.DTOs.Menu;
using Microsoft.AspNetCore.Mvc;

namespace Cafeteria.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FoodOptionController : ControllerBase
{
    private readonly IFoodOptionService _foodOptionService;

    public FoodOptionController(IFoodOptionService foodOptionService)
    {
        _foodOptionService = foodOptionService;
    }

    [HttpPost]
    public async Task<FoodOptionDto> CreateFoodOption([FromBody] FoodOptionDto foodOptionDto)
    {
        return await _foodOptionService.CreateFoodOption(foodOptionDto);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<FoodOptionDto>> GetFoodOptionByID(int id)
    {
        var result = await _foodOptionService.GetFoodOptionByID(id);
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
    public async Task<List<FoodOptionDto>> GetOptionsByEntree(int entreeId)
    {
        return await _foodOptionService.GetOptionsByEntree(entreeId);
    }

    [HttpGet("side/{sideId}")]
    public async Task<List<FoodOptionDto>> GetOptionsBySide(int sideId)
    {
        return await _foodOptionService.GetOptionsBySide(sideId);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<FoodOptionDto>> UpdateFoodOption(
        int id,
        [FromBody] FoodOptionDto foodOptionDto
    )
    {
        var result = await _foodOptionService.UpdateFoodOption(id, foodOptionDto);
        if (result == null)
            return NotFound();
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFoodOption(int id)
    {
        var result = await _foodOptionService.DeleteFoodOption(id);
        if (!result)
            return NotFound();
        return NoContent();
    }
}
