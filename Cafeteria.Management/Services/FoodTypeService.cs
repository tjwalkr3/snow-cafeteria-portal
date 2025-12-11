using Cafeteria.Shared.DTOs;

namespace Cafeteria.Management.Services;

public class FoodTypeService(IHttpClientAuth client) : IFoodTypeService
{
    public async Task<FoodOptionTypeDto> CreateFoodType(FoodOptionTypeDto foodTypeDto)
    {
        var response = await client.PostAsync("api/manager/food-types", foodTypeDto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<FoodOptionTypeDto>() ?? throw new InvalidOperationException("Failed to create food type");
    }

    public async Task<FoodOptionTypeDto?> GetFoodTypeByID(int id)
    {
        if (id < 1)
            throw new ArgumentOutOfRangeException(nameof(id));

        return await client.GetAsync<FoodOptionTypeDto>($"api/manager/food-types/{id}");
    }

    public async Task<List<FoodOptionTypeDto>> GetAllFoodTypes()
    {
        return await client.GetAsync<List<FoodOptionTypeDto>>("api/manager/food-types") ?? [];
    }

    public async Task<FoodOptionTypeDto?> UpdateFoodTypeById(int id, FoodOptionTypeDto foodTypeDto)
    {
        if (id < 1)
            throw new ArgumentOutOfRangeException(nameof(id));

        var response = await client.PutAsync($"api/manager/food-types/{id}", foodTypeDto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<FoodOptionTypeDto>();
    }

    public async Task<bool> DeleteFoodTypeById(int id)
    {
        if (id < 1)
            throw new ArgumentOutOfRangeException(nameof(id));

        var response = await client.DeleteAsync<object>($"api/manager/food-types/{id}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<bool>();
    }

    public async Task<List<EntreeDto>> GetAllEntrees()
    {
        return await client.GetAsync<List<EntreeDto>>("api/menu/entrees") ?? [];
    }

    public async Task<List<SideDto>> GetAllSides()
    {
        return await client.GetAsync<List<SideDto>>("api/menu/sides") ?? [];
    }
}
