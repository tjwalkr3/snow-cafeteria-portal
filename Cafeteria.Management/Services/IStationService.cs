using Cafeteria.Shared.DTOs;

namespace Cafeteria.Management.Services;

public interface IStationService
{
    Task<List<StationDto>> GetAllStations();
    Task<List<StationDto>> GetStationsByLocation(int locationId);
    Task<StationDto?> GetStationById(int stationId);
    Task CreateStation(int locationId, StationDto station);
    Task UpdateStation(StationDto station);
    Task DeleteStation(int stationId);
    Task<List<StationBusinessHoursDto>> GetStationBusinessHours(int stationId);
    Task<StationBusinessHoursDto?> GetStationBusinessHoursById(int hoursId);
    Task CreateStationHours(int stationId, StationBusinessHoursDto hours);
    Task UpdateStationHours(int hoursId, StationBusinessHoursDto hours);
    Task<bool> DeleteStationHoursById(int hoursId);
}
