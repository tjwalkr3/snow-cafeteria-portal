using Microsoft.AspNetCore.Mvc;
using Cafeteria.Shared.DTOsOld;
using Cafeteria.Api.Services;

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
    public async Task<List<LocationDtoOld>> GetAllLocations()
    {
        return await _menuService.GetAllLocations();
    }

    [HttpGet("stations/location/{locationId}")]
    public async Task<List<StationDtoOld>> GetStationsByLocation(int locationId)
    {
        return await _menuService.GetStationsByLocation(locationId);
    }

    [HttpGet("food-items/station/{stationId}")]
    public async Task<List<FoodItemDtoOld>> GetFoodItemsByStation(int stationId)
    {
        return await _menuService.GetFoodItemsByStation(stationId);
    }

    [HttpGet("ingredient-types/food-item/{foodItemId}")]
    public async Task<List<IngredientTypeDtoOld>> GetIngredientTypesByFoodItem(int foodItemId)
    {
        return await _menuService.GetIngredientTypesByFoodItem(foodItemId);
    }

    [HttpGet("ingredients/type/{ingredientTypeId}")]
    public async Task<List<IngredientDtoOld>> GetIngredientsByType(int ingredientTypeId)
    {
        return await _menuService.GetIngredientsByType(ingredientTypeId);
    }

    [HttpGet("ingredients/{ingredientId}")]
    public async Task<IngredientDtoOld> GetIngredientById(int ingredientId)
    {
        return await _menuService.GetIngredientById(ingredientId);
    }
}