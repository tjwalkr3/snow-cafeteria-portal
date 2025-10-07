using Microsoft.AspNetCore.Mvc;
using Cafeteria.Shared.DTOs;
using Cafeteria.Shared.Interfaces;

namespace Cafeteria.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MenuController : ControllerBase
{
    private readonly IMenuService _menuService;

    public MenuController(IMenuService menuService)
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
        return await _menuService.GetIngredientTypesByFoodItem(foodItemId);
    }

    [HttpGet("ingredients/type/{ingredientTypeId}")]
    public async Task<List<IngredientDto>> GetIngredientsByType(int ingredientTypeId)
    {
        return await _menuService.GetIngredientsByType(ingredientTypeId);
    }

    [HttpPost("ingredients/organized-by-types")]
    public async Task<Dictionary<IngredientTypeDto, List<IngredientDto>>> GetIngredientsOrganizedByType([FromBody] List<IngredientTypeDto> types)
    {
        return await _menuService.GetIngredientsOrganizedByType(types);
    }

    [HttpGet("ingredients/{ingredientId}")]
    public async Task<IngredientDto> GetIngredientById(int ingredientId)
    {
        return await _menuService.GetIngredientById(ingredientId);
    }
}