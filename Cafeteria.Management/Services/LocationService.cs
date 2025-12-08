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

    public async Task CreateLocationHours(int locationId, DateTime startTime, DateTime endTime, int weekdayId)
    {
        var response = await client.PostAsJsonAsync($"location/{locationId}/hours", new { StartTime = startTime, EndTime = endTime, WeekdayId = weekdayId });
        response.EnsureSuccessStatusCode();
    }

    public async Task UpdateLocationHours(int id, DateTime startTime, DateTime endTime, int weekdayId)
    {
        var response = await client.PutAsJsonAsync($"location/hours/{id}", new { StartTime = startTime, EndTime = endTime, WeekdayId = weekdayId });
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteLocationHours(int id)
    {
        var response = await client.DeleteAsync($"location/hours/{id}");
        response.EnsureSuccessStatusCode();
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

    public async Task DeleteLocation(int locationId)
    {
        var response = await client.DeleteAsync($"location/{locationId}");
        response.EnsureSuccessStatusCode();
    }
}
