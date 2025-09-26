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

    [HttpGet("ingredients/type/{ingredientTypeId}")]
    public async Task<ActionResult<IEnumerable<IngredientDto>>> GetIngredientsForType(int ingredientTypeId)
    {
        var ingredients = await _menuService.GetIngredientsForType(ingredientTypeId);
        return Ok(ingredients);
    }

    [HttpGet("ingredient-types/food-item/{foodItemId}")]
    public async Task<ActionResult<IEnumerable<IngredientTypeDto>>> GetIngredientTypesForFoodItem(int foodItemId)
    {
        var ingredientTypes = await _menuService.GetIngredientTypesForFoodItem(foodItemId);
        return Ok(ingredientTypes);
    }

    [HttpGet("default-ingredients/food-item/{foodItemId}")]
    public async Task<ActionResult<IEnumerable<IngredientDto>>> GetDefaultIngredientsForFoodItem(int foodItemId)
    {
        var defaultIngredients = await _menuService.GetDefaultIngredientsForFoodItem(foodItemId);
        return Ok(defaultIngredients);
    }

    [HttpGet("food-items/station/{stationId}")]
    public async Task<ActionResult<IEnumerable<FoodItemDto>>> GetFoodItemsByStation(int stationId)
    {
        var foodItems = await _menuService.GetFoodItemsByStation(stationId);
        return Ok(foodItems);
    }


    [HttpGet("food-items")]
    public async Task<ActionResult<IEnumerable<FoodItemDto>>> GetAllFoodItems()
    {
        var foodItems = await _menuService.GetAllFoodItems();
        return Ok(foodItems);
    }
}