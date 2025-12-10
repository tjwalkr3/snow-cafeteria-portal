using Cafeteria.Shared.DTOs;

namespace Cafeteria.Management.Services;

public class LocationService(IHttpClientAuth client) : ILocationService
{
    public async Task<List<LocationDto>> GetAllLocations()
    {
        return await client.GetAsync<List<LocationDto>>("api/location") ?? [];
    }

    public async Task<List<LocationBusinessHoursDto>> GetLocationBusinessHours(int locationId)
    {
        return await client.GetAsync<List<LocationBusinessHoursDto>>($"api/location/{locationId}/hours") ?? [];
    }

    public async Task CreateLocationHours(int locationId, LocationBusinessHoursDto hours)
    {
        var response = await client.PostAsync($"api/location/{locationId}/hours", hours);
        response.EnsureSuccessStatusCode();
    }

    public async Task UpdateLocationHours(int hoursId, LocationBusinessHoursDto hours)
    {
        var response = await client.PutAsync($"api/location/hours/{hoursId}", hours);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteLocationHours(int hoursId)
    {
        var response = await client.DeleteAsync<Task>($"api/location/hours/{hoursId}");
        response.EnsureSuccessStatusCode();
    }

    public async Task CreateLocation(LocationDto location)
    {
        var response = await client.PostAsync("api/location", location);
        response.EnsureSuccessStatusCode();
    }

    public async Task UpdateLocation(LocationDto location)
    {
        var response = await client.PutAsync($"api/location/{location.Id}", location);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteLocation(int locationId)
    {
        var response = await client.DeleteAsync<Task>($"api/location/{locationId}");
        response.EnsureSuccessStatusCode();
    }
}
