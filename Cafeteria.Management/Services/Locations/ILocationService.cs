using Cafeteria.Shared.DTOs.Menu;

namespace Cafeteria.Management.Services.Locations;

public interface ILocationService
{
    Task<List<LocationDto>> GetAllLocations();
    Task<List<LocationBusinessHoursDto>> GetLocationBusinessHours(int locationId);
}
