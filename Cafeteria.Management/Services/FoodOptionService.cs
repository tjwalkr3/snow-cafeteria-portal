using Cafeteria.Shared.DTOs;

namespace Cafeteria.Management.Services;

public class FoodOptionService(HttpClient client) : IFoodOptionService
{
    public async Task<FoodOptionDto> CreateFoodOption(FoodOptionDto foodOptionDto)
    {
        var response = await client.PostAsJsonAsync("manager/food-options", foodOptionDto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<FoodOptionDto>() ?? throw new InvalidOperationException("Failed to create food option");
    }

    public async Task<FoodOptionDto?> GetFoodOptionByID(int id)
    {
        if (id < 1)
            throw new ArgumentOutOfRangeException(nameof(id));

        var response = await client.GetAsync($"manager/food-options/{id}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<FoodOptionDto>();
    }

    public async Task<List<FoodOptionDto>> GetAllFoodOptions()
    {
        var response = await client.GetAsync("manager/food-options");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<FoodOptionDto>>() ?? [];
    }

    public async Task<FoodOptionDto?> UpdateFoodOptionById(int id, FoodOptionDto foodOptionDto)
    {
        if (id < 1)
            throw new ArgumentOutOfRangeException(nameof(id));

        var response = await client.PutAsJsonAsync($"manager/food-options/{id}", foodOptionDto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<FoodOptionDto>();
    }

    public async Task<bool> DeleteFoodOptionById(int id)
    {
        if (id < 1)
            throw new ArgumentOutOfRangeException(nameof(id));

        var response = await client.DeleteAsync($"manager/food-options/{id}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<bool>();
    }
}
