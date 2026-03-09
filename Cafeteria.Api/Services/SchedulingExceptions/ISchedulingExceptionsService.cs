using Cafeteria.Shared.DTOs.Menu;

namespace Cafeteria.Api.Services.SchedulingExceptions;

public interface ISchedulingExceptionsService
{
    // Location exception methods
    Task<List<LocationExceptionHoursDto>> GetLocationExceptionsByLocationId(int locationId);
    Task<LocationExceptionHoursDto?> GetLocationExceptionById(int exceptionId);
    Task AddLocationException(int locationId, DateTime startDateTime, DateTime endDateTime, string? reason = null);
    Task UpdateLocationException(int exceptionId, DateTime startDateTime, DateTime endDateTime, string? reason = null);
    Task DeleteLocationException(int exceptionId);

    // Station exception methods
    Task<List<StationExceptionHoursDto>> GetStationExceptionsByStationId(int stationId);
    Task<StationExceptionHoursDto?> GetStationExceptionById(int exceptionId);
    Task AddStationException(int stationId, DateTime startDateTime, DateTime endDateTime, string? reason = null);
    Task UpdateStationException(int exceptionId, DateTime startDateTime, DateTime endDateTime, string? reason = null);
    Task DeleteStationException(int exceptionId);
}
