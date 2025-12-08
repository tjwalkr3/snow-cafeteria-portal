using Cafeteria.Shared.DTOs;

namespace Cafeteria.Management.Services;

public class StationService(IHttpClientAuth client) : IStationService
{
    public async Task<List<StationDto>> GetAllStations()
    {
        return await client.GetAsync<List<StationDto>>("api/station") ?? [];
    }

    public async Task<List<StationDto>> GetStationsByLocation(int locationId)
    {
        return await client.GetAsync<List<StationDto>>($"api/station/station/{locationId}") ?? [];
    }

    public async Task<StationDto?> GetStationById(int stationId)
    {
        return await client.GetAsync<StationDto>($"api/station/{stationId}");
    }

    public async Task CreateStation(int locationId, StationDto station)
    {
        await client.PostAsync($"station/station/{locationId}", station);
    }

    public async Task UpdateStation(StationDto station)
    {
        await client.PutAsync($"station/{station.Id}", station);
    }

    public async Task DeleteStation(int stationId)
    {
        await client.DeleteAsync<object>($"station/{stationId}");
    }

    public async Task<List<StationBusinessHoursDto>> GetStationBusinessHours(int stationId)
    {
        return await client.GetAsync<List<StationBusinessHoursDto>>($"api/station/{stationId}/hours") ?? [];
    }

    public async Task<StationBusinessHoursDto?> GetStationBusinessHoursById(int hoursId)
    {
        return await client.GetAsync<StationBusinessHoursDto?>($"station/hours/{hoursId}");
    }

    public async Task CreateStationHours(int stationId, StationBusinessHoursDto hours)
    {
        await client.PostAsync($"station/{stationId}/hours", hours);
    }

    public async Task UpdateStationHours(int hourId, StationBusinessHoursDto hours)
    {
        await client.PutAsync($"station/hours/{hourId}", hours);
    }

    public async Task<bool> DeleteStationHoursById(int hourId)
    {
        var response = await client.DeleteAsync<object>($"station/hours/{hourId}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<bool>();
    }
}
