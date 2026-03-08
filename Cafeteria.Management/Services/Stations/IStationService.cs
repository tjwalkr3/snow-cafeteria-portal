using Cafeteria.Shared.DTOs.Menu;

namespace Cafeteria.Management.Services.Stations;

public interface IStationService
{
    Task<List<StationDto>> GetAllStations();
    Task<List<StationDto>> GetStationsByLocation(int locationId);
    Task<StationDto?> GetStationById(int stationId);
    Task<List<StationBusinessHoursDto>> GetStationBusinessHours(int stationId);
    Task AddStationBusinessHours(int stationId, TimeOnly openTime, TimeOnly closeTime, int weekdayId);
    Task UpdateStationBusinessHours(int stationHrsId, TimeOnly openTime, TimeOnly closeTime, int weekdayId);
    Task<bool> DeleteStationBusinessHours(int stationHrsId);
    Task CreateStation(int locationId, string name, string? description);
    Task UpdateStation(int stationId, string name, string? description);
    Task<bool> DeleteStation(int stationId);
}
