using Cafeteria.Shared.DTOs;

namespace Cafeteria.Management.Services;

public class FoodTypeService(IHttpClientAuth client) : IFoodTypeService
{
    public async Task<FoodOptionTypeDto> CreateFoodType(FoodOptionTypeDto foodTypeDto)
    {
        var response = await client.PostAsync("manager/food-types", foodTypeDto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<FoodOptionTypeDto>() ?? throw new InvalidOperationException("Failed to create food type");
    }

    public async Task<FoodOptionTypeDto?> GetFoodTypeByID(int id)
    {
        if (id < 1)
            throw new ArgumentOutOfRangeException(nameof(id));

        return await client.GetAsync<FoodOptionTypeDto>($"manager/food-types/{id}");
    }

    public async Task<List<FoodOptionTypeDto>> GetAllFoodTypes()
    {
        return await client.GetAsync<List<FoodOptionTypeDto>>("manager/food-types") ?? [];
    }

    public async Task<FoodOptionTypeDto?> UpdateFoodTypeById(int id, FoodOptionTypeDto foodTypeDto)
    {
        if (id < 1)
            throw new ArgumentOutOfRangeException(nameof(id));

        var response = await client.PutAsync($"manager/food-types/{id}", foodTypeDto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<FoodOptionTypeDto>();
    }

    public async Task<bool> DeleteFoodTypeById(int id)
    {
        if (id < 1)
            throw new ArgumentOutOfRangeException(nameof(id));

        var response = await client.DeleteAsync<object>($"manager/food-types/{id}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<bool>();
    }
}
