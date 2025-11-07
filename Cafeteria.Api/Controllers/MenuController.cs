using Microsoft.AspNetCore.Mvc;
using Cafeteria.Shared.DTOsOld;
using Cafeteria.Shared.DTOs;
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
    public async Task<List<FoodItemDtoOld>> GetFoodItemsByStation(int stationId)
    {
        return await _menuService.GetFoodItemsByStation(stationId);
    }

    [HttpGet("entrees/station/{stationId}")]
    public async Task<List<EntreeDto>> GetEntreesByStation(int stationId)
    {
        return await _menuService.GetEntreesByStation(stationId);
    }

    [HttpGet("sides/station/{stationId}")]
    public async Task<List<SideDto>> GetSidesByStation(int stationId)
    {
        return await _menuService.GetSidesByStation(stationId);
    }

    [HttpGet("drinks/location/{locationId}")]
    public async Task<List<DrinkDto>> GetDrinksByLocation(int locationId)
    {
        return await _menuService.GetDrinksByLocation(locationId);
    }

    [HttpGet("ingredient-types/food-item/{foodItemId}")]
    public async Task<List<IngredientTypeDtoOld>> GetIngredientTypesByFoodItem(int foodItemId)
    {
        return await _menuService.GetIngredientTypesByFoodItem(foodItemId);
    }

    [HttpGet("menu/options/entree/{entreeId}")]
    public async Task<List<FoodOptionDto>> GetFoodOptionsByEntree(int entreeId)
    {
        return await _menuService.GetOptionsByEntree(entreeId);
    }

    [HttpGet("menu/options/side/{sideId}")]
    public async Task<List<FoodOptionDto>> GetFoodOptionsBySide(int sideId)
    {
        return await _menuService.GetOptionsBySide(sideId);
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