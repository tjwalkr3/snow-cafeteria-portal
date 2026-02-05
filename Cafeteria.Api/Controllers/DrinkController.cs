using Microsoft.AspNetCore.Mvc;
using Cafeteria.Shared.DTOs.Menu;
using Cafeteria.Api.Services.Drinks;
using Microsoft.AspNetCore.Authorization;

namespace Cafeteria.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DrinkController(IDrinkService drinkService) : ControllerBase
{
    private readonly IDrinkService _drinkService = drinkService;

    [HttpGet("{id}")]
    public async Task<ActionResult<DrinkDto>> GetDrinkById(int id)
    {
        var result = await _drinkService.GetDrinkById(id);
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
    public async Task<ActionResult<List<DrinkDto>>> GetDrinksByLocationId(int locationId)
    {
        var result = await _drinkService.GetDrinksByLocationId(locationId);
        return Ok(result);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<DrinkDto>> CreateDrink([FromBody] DrinkDto drinkDto)
    {
        var result = await _drinkService.CreateDrink(drinkDto);
        return CreatedAtAction(nameof(GetDrinkById), new { id = result.Id }, result);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<ActionResult<DrinkDto>> UpdateDrinkById(int id, [FromBody] DrinkDto drinkDto)
    {
        var result = await _drinkService.UpdateDrinkById(id, drinkDto);
        if (result == null)
            return NotFound();
        return Ok(result);
    }

    [Authorize]
    [HttpPut("{id}/stock")]
    public async Task<IActionResult> SetStockStatusById(int id, [FromBody] bool inStock)
    {
        var result = await _drinkService.SetStockStatusById(id, inStock);
        if (!result)
            return NotFound();
        return NoContent();
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDrinkById(int id)
    {
        var result = await _drinkService.DeleteDrinkById(id);
        if (!result)
            return NotFound();
        return NoContent();
    }
}
