using Cafeteria.Shared.DTOs.Menu;
using Cafeteria.Shared.Services.Auth;

namespace Cafeteria.Management.Services.FoodOptions;

public class FoodOptionService(IHttpClientAuth client) : IFoodOptionService
{
    public async Task<FoodOptionDto> CreateFoodOption(FoodOptionDto foodOptionDto)
    {
        var response = await client.PostAsync("FoodOption", foodOptionDto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<FoodOptionDto>() ?? throw new InvalidOperationException("Failed to create food option");
    }

    public async Task<FoodOptionDto?> GetFoodOptionByID(int id)
    {
        if (id < 1)
            throw new ArgumentOutOfRangeException(nameof(id));

        return await client.GetAsync<FoodOptionDto>($"FoodOption/{id}");
    }

    public async Task<List<FoodOptionDto>> GetAllFoodOptions()
    {
        return await client.GetAsync<List<FoodOptionDto>>("FoodOption") ?? [];
    }

    public async Task<FoodOptionDto?> UpdateFoodOptionById(int id, FoodOptionDto foodOptionDto)
    {
        if (id < 1)
            throw new ArgumentOutOfRangeException(nameof(id));

        var response = await client.PutAsync($"FoodOption/{id}", foodOptionDto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<FoodOptionDto>();
    }

    public async Task<bool> DeleteFoodOptionById(int id)
    {
        if (id < 1)
            throw new ArgumentOutOfRangeException(nameof(id));

        var response = await client.DeleteAsync<object>($"FoodOption/{id}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<bool>();
    }
}
