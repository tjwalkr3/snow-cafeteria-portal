using Microsoft.AspNetCore.Mvc;
using Cafeteria.Api.DTOs;
using Cafeteria.Api.Services;

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
    public async Task<ActionResult<List<IngredientDto>>> GetIngredientsByType(int ingredientTypeId)
    {
        var ingredients = await _menuService.GetIngredientsByType(ingredientTypeId);
        return Ok(ingredients);
    }
}