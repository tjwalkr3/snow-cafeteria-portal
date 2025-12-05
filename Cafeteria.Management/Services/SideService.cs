using Cafeteria.Shared.DTOs;

namespace Cafeteria.Management.Services;

public class SideService : ISideService
{
    private readonly IHttpClientAuth _httpClient;

    public SideService(IHttpClientAuth httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<SideDto>> GetAllSides()
    {
        var result = await _httpClient.GetAsync<List<SideDto>>("api/side");
        return result ?? new List<SideDto>();
    }

    public async Task<bool> DeleteSide(int id)
    {
        var response = await _httpClient.DeleteAsync<object>($"api/side/{id}");
        return response?.IsSuccessStatusCode ?? false;
    }
}
