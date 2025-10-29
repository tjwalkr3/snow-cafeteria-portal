using Cafeteria.Shared.DTOsOld;
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

    public async Task<List<StationDtoOld>> GetStationsByLocation(int locationId)
    {
        if (locationId < 1)
            throw new ArgumentOutOfRangeException(nameof(locationId));

        var response = await client.GetAsync($"menu/stations/location/{locationId}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<StationDtoOld>>() ?? new List<StationDtoOld>();
    }

    public async Task<List<FoodItemDtoOld>> GetFoodItemsByStation(int stationId)
    {
        if (stationId < 1)
            throw new ArgumentOutOfRangeException(nameof(stationId));

        var response = await client.GetAsync($"menu/food-items/station/{stationId}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<FoodItemDtoOld>>() ?? new List<FoodItemDtoOld>();
    }

    public async Task<List<IngredientTypeDtoOld>> GetIngredientTypesByFoodItem(int foodItemId)
    {
        if (foodItemId < 1)
            throw new ArgumentOutOfRangeException(nameof(foodItemId));

        var response = await client.GetAsync($"menu/ingredient-types/food-item/{foodItemId}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<IngredientTypeDtoOld>>() ?? new List<IngredientTypeDtoOld>();
    }

    public async Task<List<IngredientDtoOld>> GetIngredientsByType(int ingredientTypeId)
    {
        if (ingredientTypeId < 1)
            throw new ArgumentOutOfRangeException(nameof(ingredientTypeId));

        var response = await client.GetAsync($"menu/ingredients/type/{ingredientTypeId}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<IngredientDtoOld>>() ?? new List<IngredientDtoOld>();
    }

    public async Task<IngredientDtoOld> GetIngredientById(int ingredientId)
    {
        if (ingredientId < 1)
            throw new ArgumentOutOfRangeException(nameof(ingredientId));

        var response = await client.GetAsync($"menu/ingredients/{ingredientId}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<IngredientDtoOld>() ?? new IngredientDtoOld();
    }

    public async Task<Dictionary<IngredientTypeDtoOld, List<IngredientDtoOld>>> GetIngredientsOrganizedByType(List<IngredientTypeDtoOld> types)
    {
        var result = new Dictionary<IngredientTypeDtoOld, List<IngredientDtoOld>>();

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