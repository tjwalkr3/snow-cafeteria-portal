using Cafeteria.Shared.DTOs.Menu;
using Cafeteria.Shared.Enums;

namespace Cafeteria.Api.Services.Stations;

public interface IStationService
{
    Task<List<StationDto>> GetAllStations();
    Task<List<StationDto>> GetStationsByLocationId(int locationId);
    Task<StationDto?> GetStationByID(int stationId);
    Task CreateStationByLocationId(int locationId, string stationName, string? stationDescription = null, string? iconName = null);
    Task UpdateStationById(int stationId, string name, string? description, string? iconName = null);
    Task DeleteStationById(int id);
    Task<List<StationBusinessHoursDto>> GetStationBusinessHoursByStationId(int stationId);
    Task<StationBusinessHoursDto?> GetStationBusinessHoursById(int stationHrsId);
    Task AddStationHoursByStationId(int stationId, DateTime startTime, DateTime endTime, WeekDay weekday);
    Task UpdateStationHoursById(int stationHrsId, DateTime startTime, DateTime endTime, WeekDay weekday);
    Task DeleteStationHoursById(int stationHrsId);
}
