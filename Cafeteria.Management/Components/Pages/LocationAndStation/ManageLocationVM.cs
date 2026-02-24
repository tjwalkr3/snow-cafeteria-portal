using Cafeteria.Shared.DTOs.Menu;
using Cafeteria.Management.Services.Locations;
using Cafeteria.Management.Services.Stations;
using Cafeteria.Management.Components.Pages.LocationAndStation.Location;
using Cafeteria.Management.Components.Pages.LocationAndStation.Station;

namespace Cafeteria.Management.Components.Pages.LocationAndStation;

public class ManageLocationVM : IManageLocationVM
{
    private readonly ILocationService _locationService;
    private readonly IStationService _stationService;
    private ICreateOrEditLocationVM? _createOrEditLocationVM;
    private ICreateOrEditStationVM? _createOrEditStationVM;

    public List<LocationDto> Locations { get; set; } = [];
    public Dictionary<int, List<StationDto>> StationsByLocation { get; set; } = new();

    public ManageLocationVM(ILocationService locationService, IStationService stationService)
    {
        _locationService = locationService;
        _stationService = stationService;
    }

    public void SetCreateOrEditLocationVM(ICreateOrEditLocationVM vm)
    {
        _createOrEditLocationVM = vm;
    }

    public void SetCreateOrEditStationVM(ICreateOrEditStationVM vm)
    {
        _createOrEditStationVM = vm;
    }

    public async Task LoadLocations()
    {
        Locations = await _locationService.GetAllLocations();
        StationsByLocation = new Dictionary<int, List<StationDto>>();
        foreach (var location in Locations)
        {
            StationsByLocation[location.Id] = await _stationService.GetStationsByLocation(location.Id);
        }
    }

    public async Task DeleteLocation(int id)
    {
        await _locationService.DeleteLocation(id);
        await LoadLocations();
    }

    public async Task DeleteStation(int id)
    {
        await _stationService.DeleteStation(id);
        await LoadLocations();
    }

    public Task ShowCreateLocationModal()
    {
        if (_createOrEditLocationVM != null)
        {
            _createOrEditLocationVM.CurrentLocation = new LocationDto();
            _createOrEditLocationVM.IsEditing = false;
            _createOrEditLocationVM.IsVisible = true;
        }
        return Task.CompletedTask;
    }

    public Task ShowEditLocationModal(int id)
    {
        if (_createOrEditLocationVM != null)
        {
            var location = Locations.FirstOrDefault(l => l.Id == id);
            if (location != null)
            {
                _createOrEditLocationVM.CurrentLocation = location;
                _createOrEditLocationVM.IsEditing = true;
                _createOrEditLocationVM.IsVisible = true;
            }
        }
        return Task.CompletedTask;
    }

    public async Task ShowCreateStationModal(int locationId)
    {
        if (_createOrEditStationVM != null)
        {
            _createOrEditStationVM.CurrentStation = new StationDto { LocationId = locationId };
            _createOrEditStationVM.IsEditing = false;
            _createOrEditStationVM.IsVisible = true;
            await _createOrEditStationVM.LoadLocations();
        }
    }

    public async Task ShowEditStationModal(int stationId)
    {
        if (_createOrEditStationVM != null)
        {
            StationDto? station = null;
            foreach (var stations in StationsByLocation.Values)
            {
                station = stations.FirstOrDefault(s => s.Id == stationId);
                if (station != null) break;
            }

            if (station != null)
            {
                _createOrEditStationVM.CurrentStation = station;
                _createOrEditStationVM.IsEditing = true;
                _createOrEditStationVM.IsVisible = true;
                await _createOrEditStationVM.LoadLocations();
            }
        }
    }
}
