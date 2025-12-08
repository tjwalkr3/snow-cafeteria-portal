using Cafeteria.Shared.DTOs;

namespace Cafeteria.Management.Services;

public class EntreeService(IHttpClientAuth client) : IEntreeService
{
    public async Task<List<EntreeDto>> GetAllEntrees()
    {
        return await client.GetAsync<List<EntreeDto>>("api/entree") ?? [];
    }

    public async Task<EntreeDto?> GetEntreeById(int id)
    {
        if (id < 1)
            throw new ArgumentOutOfRangeException(nameof(id));

        return await client.GetAsync<EntreeDto>($"api/entree/{id}");
    }

    public async Task<EntreeDto> CreateEntree(EntreeDto entreeDto)
    {
        var response = await client.PostAsync("api/entree", entreeDto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<EntreeDto>() ?? throw new InvalidOperationException("Failed to create entree");
    }

    public async Task<EntreeDto?> UpdateEntreeById(int id, EntreeDto entreeDto)
    {
        if (id < 1)
            throw new ArgumentOutOfRangeException(nameof(id));

        var response = await client.PutAsync($"api/entree/{id}", entreeDto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<EntreeDto>();
    }

    public async Task<bool> DeleteEntreeById(int id)
    {
        if (id < 1)
            throw new ArgumentOutOfRangeException(nameof(id));

        var response = await client.DeleteAsync<object>($"api/entree/{id}");
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> SetStockStatusById(int id, bool inStock)
    {
        if (id < 1)
            throw new ArgumentOutOfRangeException(nameof(id));

        var response = await client.PutAsync($"api/entree/{id}/stock", inStock);
        return response.IsSuccessStatusCode;
    }
}
