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
    public async Task<List<LocationDto>> GetAllLocations()
    {
        return await _menuService.GetAllLocations();
    }

    [HttpGet("stations/location/{locationId}")]
    public async Task<List<StationDto>> GetStationsByLocation(int locationId)
    {
        return await _menuService.GetStationsByLocation(locationId);
    }

    [HttpGet("food-items/station/{stationId}")]
    public async Task<List<FoodItemDto>> GetFoodItemsByStation(int stationId)
    {
        return await _menuService.GetFoodItemsByStation(stationId);
    }

    [HttpGet("ingredient-types/food-item/{foodItemId}")]
    public async Task<List<IngredientTypeDto>> GetIngredientTypesByFoodItem(int foodItemId)
    {
        return await _menuService.GetIngredientTypesForFoodItem(foodItemId);
    }

    [HttpGet("ingredients/type/{ingredientTypeId}")]
    public async Task<List<IngredientDto>> GetIngredientsByType(int ingredientTypeId)
    {
        return await _menuService.GetIngredientsOrganizedByType(ingredientTypeId);
    }
}