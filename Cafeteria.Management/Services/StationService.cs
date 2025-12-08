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

    public async Task<List<StationBusinessHoursDto>> GetStationBusinessHours(int stationId)
    {
        var response = await client.GetAsync($"station/{stationId}/hours");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<StationBusinessHoursDto>>() ?? [];
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
