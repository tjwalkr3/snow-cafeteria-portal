using Cafeteria.Shared.DTOs;

namespace Cafeteria.Management.Services;

public class LocationService(HttpClient client) : ILocationService
{
    public async Task<List<LocationDto>> GetAllLocations()
    {
        var response = await client.GetAsync("location");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<LocationDto>>() ?? [];
    }
}
