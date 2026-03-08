using Cafeteria.Shared.DTOs.Menu;
using Cafeteria.Shared.Services.Auth;

namespace Cafeteria.Management.Services.Stations;

public class StationService(IHttpClientAuth client) : IStationService
{
    public async Task<List<StationDto>> GetAllStations()
    {
        return await client.GetAsync<List<StationDto>>("station") ?? [];
    }

    public async Task<List<StationDto>> GetStationsByLocation(int locationId)
    {
        return await client.GetAsync<List<StationDto>>($"station/location/{locationId}") ?? [];
    }

    public async Task<StationDto?> GetStationById(int stationId)
    {
        return await client.GetAsync<StationDto>($"station/{stationId}");
    }

    public async Task<List<StationBusinessHoursDto>> GetStationBusinessHours(int stationId)
    {
        return await client.GetAsync<List<StationBusinessHoursDto>>($"station/{stationId}/hours") ?? [];
    }

    public async Task AddStationBusinessHours(int stationId, TimeOnly openTime, TimeOnly closeTime, int weekdayId)
    {
        var body = new
        {
            StartTime = DateTime.Today.Add(openTime.ToTimeSpan()),
            EndTime = DateTime.Today.Add(closeTime.ToTimeSpan()),
            WeekdayId = weekdayId
        };
        var response = await client.PostAsync($"station/{stationId}/hours", body);
        response.EnsureSuccessStatusCode();
    }

    public async Task UpdateStationBusinessHours(int stationHrsId, TimeOnly openTime, TimeOnly closeTime, int weekdayId)
    {
        var body = new
        {
            StartTime = DateTime.Today.Add(openTime.ToTimeSpan()),
            EndTime = DateTime.Today.Add(closeTime.ToTimeSpan()),
            WeekdayId = weekdayId
        };
        var response = await client.PutAsync($"station/hours/{stationHrsId}", body);
        response.EnsureSuccessStatusCode();
    }

    public async Task<bool> DeleteStationBusinessHours(int stationHrsId)
    {
        var response = await client.DeleteAsync<object>($"station/hours/{stationHrsId}");
        return response.IsSuccessStatusCode;
    }

    public async Task CreateStation(int locationId, string name, string? description)
    {
        var body = new { Name = name, Description = description };
        var response = await client.PostAsync($"station/location/{locationId}", body);
        response.EnsureSuccessStatusCode();
    }

    public async Task UpdateStation(int stationId, string name, string? description)
    {
        var body = new { Name = name, Description = description };
        var response = await client.PutAsync($"station/{stationId}", body);
        response.EnsureSuccessStatusCode();
    }

    public async Task<bool> DeleteStation(int stationId)
    {
        var response = await client.DeleteAsync<object>($"station/{stationId}");
        return response.IsSuccessStatusCode;
    }
}
