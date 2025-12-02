using Cafeteria.Shared.DTOs;

namespace Cafeteria.Management.Services;

public class DrinkService(HttpClient client) : IDrinkService
{
    public async Task<List<DrinkDto>> GetAllDrinks()
    {
        var response = await client.GetAsync("drink");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<DrinkDto>>() ?? [];
    }

    public async Task<DrinkDto?> GetDrinkById(int id)
    {
        if (id < 1)
            throw new ArgumentOutOfRangeException(nameof(id));

        var response = await client.GetAsync($"drink/{id}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<DrinkDto>();
    }

    public async Task<DrinkDto> CreateDrink(DrinkDto drinkDto)
    {
        var response = await client.PostAsJsonAsync("drink", drinkDto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<DrinkDto>() ?? throw new InvalidOperationException("Failed to create drink");
    }

    public async Task<DrinkDto?> UpdateDrinkById(int id, DrinkDto drinkDto)
    {
        if (id < 1)
            throw new ArgumentOutOfRangeException(nameof(id));

        var response = await client.PutAsJsonAsync($"drink/{id}", drinkDto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<DrinkDto>();
    }

    public async Task<bool> DeleteDrinkById(int id)
    {
        if (id < 1)
            throw new ArgumentOutOfRangeException(nameof(id));

        var response = await client.DeleteAsync($"drink/{id}");
        response.EnsureSuccessStatusCode();
        return true;
    }
}
