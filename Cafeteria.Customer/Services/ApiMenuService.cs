using Cafeteria.Shared.DTOs;

namespace Cafeteria.Customer.Services;

public class ApiMenuService(HttpClient client) : IApiMenuService
{
    public async Task<List<LocationDto>> GetAllLocations()
    {
        var response = await client.GetAsync("menu/locations");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<LocationDto>>() ?? new List<LocationDto>();
    }

    public async Task<List<StationDto>> GetStationsByLocation(int locationId)
    {
        if (locationId < 1)
            throw new ArgumentOutOfRangeException(nameof(locationId));

        var response = await client.GetAsync($"menu/stations/location/{locationId}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<StationDto>>() ?? new List<StationDto>();
    }

    public async Task<List<FoodItemDto>> GetFoodItemsByStation(int stationId)
    {
        if (stationId < 1)
            throw new ArgumentOutOfRangeException(nameof(stationId));

        var response = await client.GetAsync($"menu/food-items/station/{stationId}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<FoodItemDto>>() ?? new List<FoodItemDto>();
    }

    public async Task<List<IngredientTypeDto>> GetIngredientTypesByFoodItem(int foodItemId)
    {
        if (foodItemId < 1)
            throw new ArgumentOutOfRangeException(nameof(foodItemId));

        var response = await client.GetAsync($"menu/ingredient-types/food-item/{foodItemId}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<IngredientTypeDto>>() ?? new List<IngredientTypeDto>();
    }

    public async Task<List<IngredientDto>> GetIngredientsByType(int ingredientTypeId)
    {
        if (ingredientTypeId < 1)
            throw new ArgumentOutOfRangeException(nameof(ingredientTypeId));

        var response = await client.GetAsync($"menu/ingredients/type/{ingredientTypeId}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<IngredientDto>>() ?? new List<IngredientDto>();
    }

    public async Task<IngredientDto> GetIngredientById(int ingredientId)
    {
        if (ingredientId < 1)
            throw new ArgumentOutOfRangeException(nameof(ingredientId));

        var response = await client.GetAsync($"menu/ingredients/{ingredientId}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<IngredientDto>() ?? new IngredientDto();
    }

    public async Task<Dictionary<IngredientTypeDto, List<IngredientDto>>> GetIngredientsOrganizedByType(List<IngredientTypeDto> types)
    {
        var result = new Dictionary<IngredientTypeDto, List<IngredientDto>>();

        if (types == null || types.Count == 0)
            return result;

        foreach (var type in types)
        {
            var ingredients = await GetIngredientsByType(type.Id);
            result[type] = ingredients;
        }

        return result;
    }
}