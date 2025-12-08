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

    public async Task<StationDto?> GetStationById(int stationId)
    {
        var response = await client.GetAsync($"station/{stationId}");
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<StationDto>();
    }

    public async Task CreateStation(int locationId, StationDto station)
    {
        var response = await client.PostAsJsonAsync($"station/station/{locationId}", new { Name = station.StationName, Description = station.StationDescription });
        response.EnsureSuccessStatusCode();
    }

    public async Task UpdateStation(StationDto station)
    {
        var response = await client.PutAsJsonAsync($"station/{station.Id}", new { Name = station.StationName, Description = station.StationDescription });
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteStation(int stationId)
    {
        var response = await client.DeleteAsync($"station/{stationId}");
        response.EnsureSuccessStatusCode();
    }

    public async Task<List<StationBusinessHoursDto>> GetStationBusinessHours(int stationId)
    {
        return await client.GetAsync<List<StationBusinessHoursDto>>($"api/station/{stationId}/hours") ?? [];
    }

    public async Task<StationBusinessHoursDto?> GetStationBusinessHoursById(int id)
    {
        var response = await client.GetAsync($"station/hours/{id}");
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<StationBusinessHoursDto>();
    }

    public async Task CreateStationHours(int stationId, DateTime startTime, DateTime endTime, int weekdayId)
    {
        var response = await client.PostAsJsonAsync($"station/{stationId}/hours", new { StartTime = startTime, EndTime = endTime, WeekdayId = weekdayId });
        response.EnsureSuccessStatusCode();
    }

    public async Task UpdateStationHours(int id, DateTime startTime, DateTime endTime, int weekdayId)
    {
        var response = await client.PutAsJsonAsync($"station/hours/{id}", new { StartTime = startTime, EndTime = endTime, WeekdayId = weekdayId });
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteStationHours(int id)
    {
        var response = await client.DeleteAsync($"station/hours/{id}");
        response.EnsureSuccessStatusCode();
    }
}
