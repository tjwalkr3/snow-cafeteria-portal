using Cafeteria.Shared.DTOs;

namespace Cafeteria.Management.Services;

public class FoodOptionService(IHttpClientAuth client) : IFoodOptionService
{
    public async Task<FoodOptionDto> CreateFoodOption(FoodOptionDto foodOptionDto)
    {
        var response = await client.PostAsync("api/manager/food-options", foodOptionDto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<FoodOptionDto>() ?? throw new InvalidOperationException("Failed to create food option");
    }

    public async Task<FoodOptionDto?> GetFoodOptionByID(int id)
    {
        if (id < 1)
            throw new ArgumentOutOfRangeException(nameof(id));

        return await client.GetAsync<FoodOptionDto>($"api/manager/food-options/{id}");
    }

    public async Task<List<FoodOptionDto>> GetAllFoodOptions()
    {
        return await client.GetAsync<List<FoodOptionDto>>("api/manager/food-options") ?? [];
    }

    public async Task<FoodOptionDto?> UpdateFoodOptionById(int id, FoodOptionDto foodOptionDto)
    {
        if (id < 1)
            throw new ArgumentOutOfRangeException(nameof(id));

        var response = await client.PutAsync($"api/manager/food-options/{id}", foodOptionDto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<FoodOptionDto>();
    }

    public async Task<bool> DeleteFoodOptionById(int id)
    {
        if (id < 1)
            throw new ArgumentOutOfRangeException(nameof(id));

        var response = await client.DeleteAsync<object>($"api/manager/food-options/{id}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<bool>();
    }
}
