using Microsoft.AspNetCore.Mvc;
using Cafeteria.Shared.DTOs.Menu;
using Cafeteria.Api.Services.Drinks;

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

    [HttpGet("location/{locationId}")]
    public async Task<ActionResult<List<DrinkDto>>> GetDrinksByLocationID(int locationId)
    {
        var result = await _drinkService.GetDrinksByLocationID(locationId);
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

    [HttpPut("{id}/stock")]
    public async Task<IActionResult> SetStockStatusById(int id, [FromBody] bool inStock)
    {
        var result = await _drinkService.SetStockStatusById(id, inStock);
        if (!result)
            return NotFound();
        return NoContent();
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
