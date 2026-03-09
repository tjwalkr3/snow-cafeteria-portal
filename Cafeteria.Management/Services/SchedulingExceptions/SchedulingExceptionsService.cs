using Cafeteria.Shared.DTOs.Menu;
using Cafeteria.Shared.Services.Auth;

namespace Cafeteria.Management.Services.SchedulingExceptions;

public class SchedulingExceptionsService(IHttpClientAuth client) : ISchedulingExceptionsService
{
    // Location exceptions
    public async Task<List<LocationExceptionHoursDto>> GetLocationExceptions(int locationId)
    {
        return await client.GetAsync<List<LocationExceptionHoursDto>>($"SchedulingExceptions/location/{locationId}") ?? [];
    }

    public async Task AddLocationException(int locationId, DateTime startDateTime, DateTime endDateTime, string? reason = null)
    {
        var body = new { StartDateTime = startDateTime, EndDateTime = endDateTime, Reason = reason };
        var response = await client.PostAsync($"SchedulingExceptions/location/{locationId}", body);
        response.EnsureSuccessStatusCode();
    }

    public async Task UpdateLocationException(int exceptionId, DateTime startDateTime, DateTime endDateTime, string? reason = null)
    {
        var body = new { StartDateTime = startDateTime, EndDateTime = endDateTime, Reason = reason };
        var response = await client.PutAsync($"SchedulingExceptions/location/{exceptionId}", body);
        response.EnsureSuccessStatusCode();
    }

    public async Task<bool> DeleteLocationException(int exceptionId)
    {
        var response = await client.DeleteAsync<object>($"SchedulingExceptions/location/{exceptionId}");
        return response.IsSuccessStatusCode;
    }

    // Station exceptions
    public async Task<List<StationExceptionHoursDto>> GetStationExceptions(int stationId)
    {
        return await client.GetAsync<List<StationExceptionHoursDto>>($"SchedulingExceptions/station/{stationId}") ?? [];
    }

    public async Task AddStationException(int stationId, DateTime startDateTime, DateTime endDateTime, string? reason = null)
    {
        var body = new { StartDateTime = startDateTime, EndDateTime = endDateTime, Reason = reason };
        var response = await client.PostAsync($"SchedulingExceptions/station/{stationId}", body);
        response.EnsureSuccessStatusCode();
    }

    public async Task UpdateStationException(int exceptionId, DateTime startDateTime, DateTime endDateTime, string? reason = null)
    {
        var body = new { StartDateTime = startDateTime, EndDateTime = endDateTime, Reason = reason };
        var response = await client.PutAsync($"SchedulingExceptions/station/{exceptionId}", body);
        response.EnsureSuccessStatusCode();
    }

    public async Task<bool> DeleteStationException(int exceptionId)
    {
        var response = await client.DeleteAsync<object>($"SchedulingExceptions/station/{exceptionId}");
        return response.IsSuccessStatusCode;
    }
}
