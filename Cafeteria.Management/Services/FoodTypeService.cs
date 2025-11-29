using Cafeteria.Shared.DTOs;

namespace Cafeteria.Management.Services;

public class FoodTypeService(HttpClient client) : IFoodTypeService
{
    public async Task<FoodOptionTypeDto> CreateFoodType(FoodOptionTypeDto foodTypeDto)
    {
        var response = await client.PostAsJsonAsync("manager/food-types", foodTypeDto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<FoodOptionTypeDto>() ?? throw new InvalidOperationException("Failed to create food type");
    }

    public async Task<FoodOptionTypeDto?> GetFoodTypeByID(int id)
    {
        if (id < 1)
            throw new ArgumentOutOfRangeException(nameof(id));

        var response = await client.GetAsync($"manager/food-types/{id}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<FoodOptionTypeDto>();
    }

    public async Task<List<FoodOptionTypeDto>> GetAllFoodTypes()
    {
        var response = await client.GetAsync("manager/food-types");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<FoodOptionTypeDto>>() ?? [];
    }

    public async Task<FoodOptionTypeDto?> UpdateFoodTypeById(int id, FoodOptionTypeDto foodTypeDto)
    {
        if (id < 1)
            throw new ArgumentOutOfRangeException(nameof(id));

        var response = await client.PutAsJsonAsync($"manager/food-types/{id}", foodTypeDto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<FoodOptionTypeDto>();
    }

    public async Task<bool> DeleteFoodTypeById(int id)
    {
        if (id < 1)
            throw new ArgumentOutOfRangeException(nameof(id));

        var response = await client.DeleteAsync($"manager/food-types/{id}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<bool>();
    }
}
