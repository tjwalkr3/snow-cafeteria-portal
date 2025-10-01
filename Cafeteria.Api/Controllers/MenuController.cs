using Microsoft.AspNetCore.Mvc;
using Cafeteria.Api.Services;
using Cafeteria.Shared.DTOs;

namespace Cafeteria.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MenuController : ControllerBase
{

    private readonly MenuService _menuService;

    public MenuController(MenuService menuService)
    {
        _menuService = menuService;
    }

    [HttpGet("locations")]
    public async Task<ActionResult<List<LocationDto>>> GetAllLocations()
    {
        var locations = await _menuService.GetAllLocations();
        return Ok(locations);
    }

    [HttpGet("stations/location/{locationId}")]
    public async Task<ActionResult<List<StationDto>>> GetStationsByLocation(int locationId)
    {
        var stations = await _menuService.GetStationsByLocation(locationId);
        return Ok(stations);
    }

    [HttpGet("food-items/station/{stationId}")]
    public async Task<ActionResult<List<FoodItemDto>>> GetFoodItemsByStation(int stationId)
    {
        var foodItems = await _menuService.GetFoodItemsByStation(stationId);
        return Ok(foodItems);
    }

    [HttpGet("ingredient-types/food-item/{foodItemId}")]
    public async Task<ActionResult<List<IngredientTypeDto>>> GetIngredientTypesByFoodItem(int foodItemId)
    {
        var ingredientTypes = await _menuService.GetIngredientTypesByFoodItem(foodItemId);
        return Ok(ingredientTypes);
    }

    [HttpGet("ingredients/type/{ingredientTypeId}")]
    public async Task<ActionResult<List<IngredientDto>>> GetIngredientsForTypes(int ingredientTypeId)
    {
        var ingredients = await _menuService.GetIngredientsForTypes(ingredientTypeId);
        return Ok(ingredients);
    }
}