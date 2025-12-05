using Cafeteria.Shared.DTOs;

namespace Cafeteria.Management.Services;

public interface IStationService
{
    Task<List<StationDto>> GetStationsByLocation(int locationId);
    Task<StationDto?> GetStationById(int stationId);
    Task<List<StationBusinessHoursDto>> GetStationBusinessHours(int stationId);
}
