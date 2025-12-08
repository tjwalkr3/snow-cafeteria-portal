using Cafeteria.Management.Services;
using Cafeteria.Shared.DTOs;

namespace Cafeteria.Management.Components.Pages.Side;

public class CreateOrEditSideVM : ICreateOrEditSideVM
{
    private readonly ISideService _sideService;
    private readonly ILocationService _locationService;
    private readonly IStationService _stationService;

    public CreateOrEditSideVM(ISideService sideService, ILocationService locationService, IStationService stationService)
    {
        _sideService = sideService;
        _locationService = locationService;
        _stationService = stationService;
    }

    public async Task CreateSideAsync(SideDto side)
    {
        await _sideService.CreateSide(side);
    }

    public async Task UpdateSideAsync(SideDto side)
    {
        await _sideService.UpdateSide(side);
    }

    public async Task<List<LocationDto>> GetLocationsAsync()
    {
        return await _locationService.GetAllLocations();
    }

    public async Task<List<StationDto>> GetStationsByLocationAsync(int locationId)
    {
        return await _stationService.GetStationsByLocation(locationId);
    }

    public async Task<StationDto?> GetStationByIdAsync(int stationId)
    {
        return await _stationService.GetStationById(stationId);
    }
}
