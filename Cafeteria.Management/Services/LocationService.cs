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

    public async Task<List<LocationBusinessHoursDto>> GetLocationBusinessHours(int locationId)
    {
        var response = await client.GetAsync($"location/{locationId}/hours");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<LocationBusinessHoursDto>>() ?? [];
    }

    public async Task CreateLocation(LocationDto location)
    {
        var response = await client.PostAsJsonAsync("location", new { Name = location.LocationName, Description = location.LocationDescription });
        response.EnsureSuccessStatusCode();
    }

    public async Task UpdateLocation(LocationDto location)
    {
        var response = await client.PutAsJsonAsync($"location/{location.Id}", new { Name = location.LocationName, Description = location.LocationDescription });
        response.EnsureSuccessStatusCode();
    }
}
