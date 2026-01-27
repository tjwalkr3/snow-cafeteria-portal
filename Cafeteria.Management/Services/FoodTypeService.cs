using Cafeteria.Shared.DTOs.Menu;

namespace Cafeteria.Management.Services;

public class FoodTypeService(IHttpClientAuth client) : IFoodTypeService
{
    public async Task<FoodOptionTypeDto> CreateFoodType(FoodOptionTypeDto foodTypeDto)
    {
        var response = await client.PostAsync("api/FoodOptionType", foodTypeDto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<FoodOptionTypeDto>() ?? throw new InvalidOperationException("Failed to create food type");
    }

    public async Task<FoodOptionTypeDto?> GetFoodTypeByID(int id)
    {
        if (id < 1)
            throw new ArgumentOutOfRangeException(nameof(id));

        return await client.GetAsync<FoodOptionTypeDto>($"api/FoodOptionType/{id}");
    }

    public async Task<List<FoodOptionTypeDto>> GetAllFoodTypes()
    {
        return await client.GetAsync<List<FoodOptionTypeDto>>("api/FoodOptionType") ?? [];
    }

    public async Task<FoodOptionTypeDto?> UpdateFoodTypeById(int id, FoodOptionTypeDto foodTypeDto)
    {
        if (id < 1)
            throw new ArgumentOutOfRangeException(nameof(id));

        var response = await client.PutAsync($"api/FoodOptionType/{id}", foodTypeDto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<FoodOptionTypeDto>();
    }

    public async Task<bool> DeleteFoodTypeById(int id)
    {
        if (id < 1)
            throw new ArgumentOutOfRangeException(nameof(id));

        var response = await client.DeleteAsync<object>($"api/FoodOptionType/{id}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<bool>();
    }

    public async Task<List<EntreeDto>> GetAllEntrees()
    {
        return await client.GetAsync<List<EntreeDto>>("api/entree") ?? [];
    }

    public async Task<List<SideDto>> GetAllSides()
    {
        return await client.GetAsync<List<SideDto>>("api/side") ?? [];
    }
}
