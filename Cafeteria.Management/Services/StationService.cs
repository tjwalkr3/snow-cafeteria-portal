using Cafeteria.Shared.DTOs;

namespace Cafeteria.Management.Services;

public class StationService(IHttpClientAuth client) : IStationService
{
    public async Task<List<StationDto>> GetStationsByLocation(int locationId)
    {
        return await client.GetAsync<List<StationDto>>($"api/station/station/{locationId}") ?? [];
    }

    public async Task<StationDto?> GetStationById(int stationId)
    {
        return await client.GetAsync<StationDto>($"api/station/{stationId}");
    }

    public async Task<List<StationBusinessHoursDto>> GetStationBusinessHours(int stationId)
    {
        return await client.GetAsync<List<StationBusinessHoursDto>>($"api/station/{stationId}/hours") ?? [];
    }
}
