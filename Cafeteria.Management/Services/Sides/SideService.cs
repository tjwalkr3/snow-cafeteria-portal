using Cafeteria.Shared.DTOs.Menu;
using Cafeteria.Shared.Services.Auth;

namespace Cafeteria.Management.Services.Sides;

public class SideService : ISideService
{
    private readonly IHttpClientAuth _httpClient;

    public SideService(IHttpClientAuth httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<SideDto>> GetAllSides()
    {
        var result = await _httpClient.GetAsync<List<SideDto>>("side");
        return result ?? new List<SideDto>();
    }

    public async Task<SideDto?> GetSideById(int id)
    {
        return await _httpClient.GetAsync<SideDto>($"side/{id}");
    }

    public async Task<SideDto?> CreateSide(SideDto side)
    {
        var response = await _httpClient.PostAsync("Side", side);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<SideDto>();
        }

        var error = await response.Content.ReadAsStringAsync();
        throw new Exception(string.IsNullOrWhiteSpace(error) ? "Failed to create side." : error);
    }

    public async Task<SideDto?> UpdateSide(SideDto side)
    {
        var response = await _httpClient.PutAsync($"Side/{side.Id}", side);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<SideDto>();
        }

        var error = await response.Content.ReadAsStringAsync();
        throw new Exception(string.IsNullOrWhiteSpace(error) ? "Failed to update side." : error);
    }

    public async Task<bool> DeleteSide(int id)
    {
        var response = await _httpClient.DeleteAsync<object>($"side/{id}");
        return response?.IsSuccessStatusCode ?? false;
    }

    public async Task<bool> SetStockStatusById(int id, bool inStock)
    {
        var response = await _httpClient.PutAsync($"side/{id}/stock", inStock);
        return response.IsSuccessStatusCode;
    }
}
