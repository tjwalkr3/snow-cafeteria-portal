using Cafeteria.Shared.DTOs.Menu;
using Cafeteria.Management.Services.Auth;

namespace Cafeteria.Management.Services.FoodOptions;

public class FoodOptionService(IHttpClientAuth client) : IFoodOptionService
{
    public async Task<FoodOptionDto> CreateFoodOption(FoodOptionDto foodOptionDto)
    {
        var response = await client.PostAsync("api/FoodOption", foodOptionDto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<FoodOptionDto>() ?? throw new InvalidOperationException("Failed to create food option");
    }

    public async Task<FoodOptionDto?> GetFoodOptionByID(int id)
    {
        if (id < 1)
            throw new ArgumentOutOfRangeException(nameof(id));

        return await client.GetAsync<FoodOptionDto>($"api/FoodOption/{id}");
    }

    public async Task<List<FoodOptionDto>> GetAllFoodOptions()
    {
        return await client.GetAsync<List<FoodOptionDto>>("api/FoodOption") ?? [];
    }

    public async Task<FoodOptionDto?> UpdateFoodOptionById(int id, FoodOptionDto foodOptionDto)
    {
        if (id < 1)
            throw new ArgumentOutOfRangeException(nameof(id));

        var response = await client.PutAsync($"api/FoodOption/{id}", foodOptionDto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<FoodOptionDto>();
    }

    public async Task<bool> DeleteFoodOptionById(int id)
    {
        if (id < 1)
            throw new ArgumentOutOfRangeException(nameof(id));

        var response = await client.DeleteAsync<object>($"api/FoodOption/{id}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<bool>();
    }
}
