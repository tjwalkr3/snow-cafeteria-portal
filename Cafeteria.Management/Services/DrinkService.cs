using Cafeteria.Shared.DTOs;

namespace Cafeteria.Management.Services;

public class DrinkService(IHttpClientAuth client) : IDrinkService
{
    public async Task<List<DrinkDto>> GetAllDrinks()
    {
        return await client.GetAsync<List<DrinkDto>>("api/drink") ?? [];
    }

    public async Task<DrinkDto?> GetDrinkById(int id)
    {
        if (id < 1)
            throw new ArgumentOutOfRangeException(nameof(id));

        return await client.GetAsync<DrinkDto>($"api/drink/{id}");
    }

    public async Task<DrinkDto> CreateDrink(DrinkDto drinkDto)
    {
        var response = await client.PostAsync("api/drink", drinkDto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<DrinkDto>() ?? throw new InvalidOperationException("Failed to create drink");
    }

    public async Task<DrinkDto?> UpdateDrinkById(int id, DrinkDto drinkDto)
    {
        if (id < 1)
            throw new ArgumentOutOfRangeException(nameof(id));

        var response = await client.PutAsync($"api/drink/{id}", drinkDto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<DrinkDto>();
    }

    public async Task<bool> DeleteDrinkById(int id)
    {
        if (id < 1)
            throw new ArgumentOutOfRangeException(nameof(id));

        var response = await client.DeleteAsync<object>($"api/drink/{id}");
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> SetStockStatusById(int id, bool inStock)
    {
        if (id < 1)
            throw new ArgumentOutOfRangeException(nameof(id));

        var response = await client.PutAsync($"api/drink/{id}/stock", inStock);
        return response.IsSuccessStatusCode;
    }
}
