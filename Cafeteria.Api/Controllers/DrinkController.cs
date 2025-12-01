using Microsoft.AspNetCore.Mvc;
using Cafeteria.Shared.DTOs;
using Cafeteria.Api.Services;

namespace Cafeteria.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DrinkController : ControllerBase
{
    private readonly IDrinkService _drinkService;

    public DrinkController(IDrinkService drinkService)
    {
        _drinkService = drinkService;
    }

    [HttpPost]
    public async Task<ActionResult<DrinkDto>> CreateDrink([FromBody] DrinkDto drinkDto)
    {
        var result = await _drinkService.CreateDrink(drinkDto);
        return CreatedAtAction(nameof(GetDrinkByID), new { id = result.Id }, result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DrinkDto>> GetDrinkByID(int id)
    {
        var result = await _drinkService.GetDrinkByID(id);
        if (result == null)
            return NotFound();
        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<List<DrinkDto>>> GetAllDrinks()
    {
        var result = await _drinkService.GetAllDrinks();
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<DrinkDto>> UpdateDrinkByID(int id, [FromBody] DrinkDto drinkDto)
    {
        var result = await _drinkService.UpdateDrinkByID(id, drinkDto);
        if (result == null)
            return NotFound();
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDrinkByID(int id)
    {
        var result = await _drinkService.DeleteDrinkByID(id);
        if (!result)
            return NotFound();
        return NoContent();
    }
}
