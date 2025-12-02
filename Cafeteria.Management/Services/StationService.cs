using Cafeteria.Shared.DTOs;

namespace Cafeteria.Management.Services;

public class StationService(HttpClient client) : IStationService
{
    public async Task<List<StationDto>> GetStationsByLocation(int locationId)
    {
        var response = await client.GetAsync($"station/station/{locationId}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<StationDto>>() ?? [];
    }
}
