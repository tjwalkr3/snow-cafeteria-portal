using Cafeteria.Shared.DTOs.Menu;

namespace Cafeteria.Management.Services.Stations;

public interface IStationService
{
    Task<List<StationDto>> GetAllStations();
    Task<List<StationDto>> GetStationsByLocation(int locationId);
    Task<StationDto?> GetStationById(int stationId);
    Task<List<StationBusinessHoursDto>> GetStationBusinessHours(int stationId);
}
