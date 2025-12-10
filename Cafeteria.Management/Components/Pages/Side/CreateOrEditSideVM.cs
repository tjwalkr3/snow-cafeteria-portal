using Cafeteria.Management.Services;
using Cafeteria.Shared.DTOs;

namespace Cafeteria.Management.Components.Pages.Side;

public class CreateOrEditSideVM : ICreateOrEditSideVM
{
    private readonly ISideService _sideService;
    private readonly ILocationService _locationService;
    private readonly IStationService _stationService;
    private readonly ISideVM _sideVM;

    public CreateOrEditSideVM(ISideService sideService, ILocationService locationService, IStationService stationService, ISideVM sideVM)
    {
        _sideService = sideService;
        _locationService = locationService;
        _stationService = stationService;
        _sideVM = sideVM;
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

    public bool ValidateSide(IEnumerable<SideDto> existingSides, SideDto newSide)
    {
        return !existingSides.Any(s => 
            s.SideName.Equals(newSide.SideName, StringComparison.OrdinalIgnoreCase) && 
            s.StationId == newSide.StationId &&
            s.Id != newSide.Id);
    }
}
