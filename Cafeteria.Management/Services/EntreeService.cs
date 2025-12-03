using Cafeteria.Shared.DTOs;

namespace Cafeteria.Management.Services;

public class EntreeService(HttpClient client) : IEntreeService
{
    public async Task<List<EntreeDto>> GetAllEntrees()
    {
        var response = await client.GetAsync("entree");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<EntreeDto>>() ?? [];
    }

    public async Task<EntreeDto?> GetEntreeById(int id)
    {
        if (id < 1)
            throw new ArgumentOutOfRangeException(nameof(id));

        var response = await client.GetAsync($"entree/{id}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<EntreeDto>();
    }

    public async Task<EntreeDto> CreateEntree(EntreeDto entreeDto)
    {
        var response = await client.PostAsJsonAsync("entree", entreeDto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<EntreeDto>() ?? throw new InvalidOperationException("Failed to create entree");
    }

    public async Task<EntreeDto?> UpdateEntreeById(int id, EntreeDto entreeDto)
    {
        if (id < 1)
            throw new ArgumentOutOfRangeException(nameof(id));

        var response = await client.PutAsJsonAsync($"entree/{id}", entreeDto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<EntreeDto>();
    }

    public async Task<bool> DeleteEntreeById(int id)
    {
        if (id < 1)
            throw new ArgumentOutOfRangeException(nameof(id));

        var response = await client.DeleteAsync($"entree/{id}");
        response.EnsureSuccessStatusCode();
        return true;
    }
}
