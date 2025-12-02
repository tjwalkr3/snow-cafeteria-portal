using Cafeteria.Shared.DTOs;

namespace Cafeteria.Management.Services;

public interface ILocationService
{
    Task<List<LocationDto>> GetAllLocations();
}
