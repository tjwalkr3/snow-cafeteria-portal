using Cafeteria.Shared.DTOs;
using Cafeteria.Shared.Interfaces;

namespace Cafeteria.Customer.Services;

public class ApiMenuService(HttpClient client) : IApiMenuService
{
    public async Task<List<LocationDto>> GetAllLocations()
    {
        var response = client.GetAsync("/locations").Result;
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<LocationDto>>() ?? new List<LocationDto>();
    }

    public async Task<List<StationDto>> GetStationsByLocation(int locationId)
    {
        if (locationId < 1)
            throw new ArgumentOutOfRangeException(nameof(locationId));

        var response = client.GetAsync($"/stations/location/{locationId}").Result;
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<StationDto>>() ?? new List<StationDto>();
    }

    public async Task<List<FoodItemDto>> GetFoodItemsByStation(int stationId)
    {
        if (stationId < 1)
            throw new ArgumentOutOfRangeException(nameof(stationId));

        var response = client.GetAsync($"/food-items/station/{stationId}").Result;
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<FoodItemDto>>() ?? new List<FoodItemDto>();
    }

    public async Task<List<IngredientTypeDto>> GetIngredientTypesByFoodItem(int foodItemId)
    {
        if (foodItemId < 1)
            throw new ArgumentOutOfRangeException(nameof(foodItemId));

        var response = client.GetAsync($"/ingredient-types/food-item/{foodItemId}").Result;
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<IngredientTypeDto>>() ?? new List<IngredientTypeDto>();
    }

    public async Task<List<IngredientDto>> GetIngredientsByType(int ingredientTypeId)
    {
        if (ingredientTypeId < 1)
            throw new ArgumentOutOfRangeException(nameof(ingredientTypeId));

        var response = client.GetAsync($"/ingredients/type/{ingredientTypeId}").Result;
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<IngredientDto>>() ?? new List<IngredientDto>();
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

    public async Task<IngredientDto> GetIngredientById(int ingredientId)
    {
        if (ingredientId < 1)
            throw new ArgumentOutOfRangeException(nameof(ingredientId));

        var response = client.GetAsync($"/ingredients/{ingredientId}").Result;
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<IngredientDto>() ?? new IngredientDto();
    }
}