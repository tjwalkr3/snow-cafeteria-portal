using Cafeteria.Shared.DTOs;

namespace Cafeteria.Management.Components.Pages.Side;

public interface ICreateOrEditSideVM
{
    Task CreateSideAsync(SideDto side);
    Task UpdateSideAsync(SideDto side);
    Task<List<LocationDto>> GetLocationsAsync();
    Task<List<StationDto>> GetStationsByLocationAsync(int locationId);
    Task<StationDto?> GetStationByIdAsync(int stationId);
    bool ValidateSide(IEnumerable<SideDto> existingSides, SideDto newSide);
}
