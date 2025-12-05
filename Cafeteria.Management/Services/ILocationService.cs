using Cafeteria.Shared.DTOs;

namespace Cafeteria.Management.Services;

public interface ILocationService
{
    Task<List<LocationDto>> GetAllLocations();
    Task<List<LocationBusinessHoursDto>> GetLocationBusinessHours(int locationId);
    Task CreateLocation(LocationDto location);
    Task UpdateLocation(LocationDto location);
    Task DeleteLocation(int locationId);
}
