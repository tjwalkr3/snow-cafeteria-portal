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
}
