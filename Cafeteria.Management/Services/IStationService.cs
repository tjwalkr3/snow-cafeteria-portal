using Cafeteria.Shared.DTOs;

namespace Cafeteria.Management.Services;

public interface IStationService
{
    Task<List<StationDto>> GetStationsByLocation(int locationId);
    Task<List<StationBusinessHoursDto>> GetStationBusinessHours(int stationId);
    Task<StationBusinessHoursDto?> GetStationBusinessHoursById(int id);
    Task CreateStationHours(int stationId, DateTime startTime, DateTime endTime, int weekdayId);
    Task UpdateStationHours(int id, DateTime startTime, DateTime endTime, int weekdayId);
    Task DeleteStationHours(int id);
}
