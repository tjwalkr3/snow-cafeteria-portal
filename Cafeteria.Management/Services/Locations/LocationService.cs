using Cafeteria.Shared.DTOs.Menu;
using Cafeteria.Shared.Services.Auth;

namespace Cafeteria.Management.Services.Locations;

public class LocationService(IHttpClientAuth client) : ILocationService
{
    public async Task<List<LocationDto>> GetAllLocations()
    {
        return await client.GetAsync<List<LocationDto>>("location") ?? [];
    }

    public async Task<List<LocationBusinessHoursDto>> GetLocationBusinessHours(int locationId)
    {
        return await client.GetAsync<List<LocationBusinessHoursDto>>($"location/{locationId}/hours") ?? [];
    }

    public async Task AddLocationBusinessHours(int locationId, TimeOnly openTime, TimeOnly closeTime, int weekdayId)
    {
        var body = new
        {
            StartTime = DateTime.Today.Add(openTime.ToTimeSpan()),
            EndTime = DateTime.Today.Add(closeTime.ToTimeSpan()),
            WeekdayId = weekdayId
        };
        var response = await client.PostAsync($"location/{locationId}/hours", body);
        response.EnsureSuccessStatusCode();
    }

    public async Task UpdateLocationBusinessHours(int locationHrsId, TimeOnly openTime, TimeOnly closeTime, int weekdayId)
    {
        var body = new
        {
            StartTime = DateTime.Today.Add(openTime.ToTimeSpan()),
            EndTime = DateTime.Today.Add(closeTime.ToTimeSpan()),
            WeekdayId = weekdayId
        };
        var response = await client.PutAsync($"location/hours/{locationHrsId}", body);
        response.EnsureSuccessStatusCode();
    }

    public async Task<bool> DeleteLocationBusinessHours(int locationHrsId)
    {
        var response = await client.DeleteAsync<object>($"location/hours/{locationHrsId}");
        return response.IsSuccessStatusCode;
    }
}
