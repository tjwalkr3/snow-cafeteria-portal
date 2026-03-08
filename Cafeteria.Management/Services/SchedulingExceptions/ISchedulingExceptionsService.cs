using Cafeteria.Shared.DTOs.Menu;

namespace Cafeteria.Management.Services.SchedulingExceptions;

public interface ISchedulingExceptionsService
{
    // Location exceptions
    Task<List<LocationExceptionHoursDto>> GetLocationExceptions(int locationId);
    Task AddLocationException(int locationId, DateTime startDateTime, DateTime endDateTime, string? reason = null);
    Task UpdateLocationException(int exceptionId, DateTime startDateTime, DateTime endDateTime, string? reason = null);
    Task<bool> DeleteLocationException(int exceptionId);

    // Station exceptions
    Task<List<StationExceptionHoursDto>> GetStationExceptions(int stationId);
    Task AddStationException(int stationId, DateTime startDateTime, DateTime endDateTime, string? reason = null);
    Task UpdateStationException(int exceptionId, DateTime startDateTime, DateTime endDateTime, string? reason = null);
    Task<bool> DeleteStationException(int exceptionId);
}
