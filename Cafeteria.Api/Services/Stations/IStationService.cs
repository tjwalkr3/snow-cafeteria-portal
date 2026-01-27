using Cafeteria.Shared.DTOs.Menu;
using Cafeteria.Shared.Enums;

namespace Cafeteria.Api.Services.Stations;

public interface IStationService
{
    Task<List<StationDto>> GetAllStations();
    Task<List<StationDto>> GetStationsByLocation(int locationId);
    Task<StationDto?> GetStationByID(int stationId);
    Task CreateStationForLocation(int locationId, string stationName, string? stationDescription = null);
    Task UpdateStationByID(int stationId, string name, string? description);
    Task DeleteStationByID(int id);
    Task<List<StationBusinessHoursDto>> GetStationBusinessHours(int stationId);
    Task<StationBusinessHoursDto?> GetStationBusinessHoursById(int stationHrsId);
    Task AddStationHours(int stationId, DateTime startTime, DateTime endTime, WeekDay weekday);
    Task UpdateStationHoursById(int stationHrsId, DateTime startTime, DateTime endTime, WeekDay weekday);
    Task DeleteStationHrsById(int stationHrsId);
}
