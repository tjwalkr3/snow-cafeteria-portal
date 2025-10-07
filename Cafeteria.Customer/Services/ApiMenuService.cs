using Cafeteria.Shared.DTOs;

namespace Cafeteria.Customer.Services;

public class ApiMenuService(HttpClient client) : IApiMenuService
{
    public List<LocationDto> GetAllLocations()
    {
        var response = client.GetAsync("/locations").Result;
        response.EnsureSuccessStatusCode();
        return response.Content.ReadFromJsonAsync<List<LocationDto>>().Result ?? new List<LocationDto>();
    }

    public List<StationDto> GetStationsByLocation(int locationId)
    {
        if (locationId < 1)
            throw new ArgumentOutOfRangeException(nameof(locationId));
        
        var response = client.GetAsync($"/stations/location/{locationId}").Result;
        response.EnsureSuccessStatusCode();
        return response.Content.ReadFromJsonAsync<List<StationDto>>().Result ?? new List<StationDto>();
    }

    public List<FoodItemDto> GetFoodItemsByStation(int stationId)
    {
        if (stationId < 1)
            throw new ArgumentOutOfRangeException(nameof(stationId));
        
        var response = client.GetAsync($"/food-items/station/{stationId}").Result;
        response.EnsureSuccessStatusCode();
        return response.Content.ReadFromJsonAsync<List<FoodItemDto>>().Result ?? new List<FoodItemDto>();
    }

    public List<IngredientTypeDto> GetIngredientTypesByFoodItem(int foodItemId)
    {
        if (foodItemId < 1)
            throw new ArgumentOutOfRangeException(nameof(foodItemId));
        
        var response = client.GetAsync($"/ingredient-types/food-item/{foodItemId}").Result;
        response.EnsureSuccessStatusCode();
        return response.Content.ReadFromJsonAsync<List<IngredientTypeDto>>().Result ?? new List<IngredientTypeDto>();
    }

    public List<IngredientDto> GetIngredientsByType(int ingredientTypeId)
    {
        if (ingredientTypeId < 1)
            throw new ArgumentOutOfRangeException(nameof(ingredientTypeId));
        
        var response = client.GetAsync($"/ingredients/type/{ingredientTypeId}").Result;
        response.EnsureSuccessStatusCode();
        return response.Content.ReadFromJsonAsync<List<IngredientDto>>().Result ?? new List<IngredientDto>();
    }
}