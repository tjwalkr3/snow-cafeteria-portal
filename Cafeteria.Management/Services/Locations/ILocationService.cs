using Cafeteria.Shared.DTOs.Menu;

namespace Cafeteria.Management.Services.Locations;

public interface ILocationService
{
    Task<List<LocationDto>> GetAllLocations();
    Task<List<LocationBusinessHoursDto>> GetLocationBusinessHours(int locationId);
    Task AddLocationBusinessHours(int locationId, TimeOnly openTime, TimeOnly closeTime, int weekdayId);
    Task UpdateLocationBusinessHours(int locationHrsId, TimeOnly openTime, TimeOnly closeTime, int weekdayId);
    Task<bool> DeleteLocationBusinessHours(int locationHrsId);
}
