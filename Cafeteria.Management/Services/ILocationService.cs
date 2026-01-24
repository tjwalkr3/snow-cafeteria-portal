using Cafeteria.Shared.DTOs.Menu;

namespace Cafeteria.Management.Services;

public interface ILocationService
{
    Task<List<LocationDto>> GetAllLocations();
    Task<List<LocationBusinessHoursDto>> GetLocationBusinessHours(int locationId);
}
