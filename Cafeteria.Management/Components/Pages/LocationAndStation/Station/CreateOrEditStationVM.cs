using Cafeteria.Shared.DTOs.Menu;
using Cafeteria.Management.Services.Stations;
using Cafeteria.Management.Services.Locations;
using Cafeteria.Management.Components.Shared;

namespace Cafeteria.Management.Components.Pages.LocationAndStation.Station;

public class CreateOrEditStationVM : ICreateOrEditStationVM
{
    private readonly IStationService _stationService;
    private readonly ILocationService _locationService;
    private readonly IManageLocationVM _parentVM;

    public StationDto CurrentStation { get; set; } = new();
    public List<LocationDto> Locations { get; set; } = [];
    public bool IsVisible { get; set; }
    public bool IsEditing { get; set; }
    public bool ShowToast { get; set; }
    public string ToastMessage { get; set; } = string.Empty;
    public Toast.ToastType ToastType { get; set; }

    public CreateOrEditStationVM(IStationService stationService, ILocationService locationService, IManageLocationVM parentVM)
    {
        _stationService = stationService;
        _locationService = locationService;
        _parentVM = parentVM;
    }

    public async Task LoadLocations()
    {
        Locations = await _locationService.GetAllLocations();
    }

    public async Task<bool> SaveStation()
    {
        var stationsInLocation = _parentVM.StationsByLocation.TryGetValue(CurrentStation.LocationId, out var stations)
            ? stations
            : new List<StationDto>();

        var isDuplicate = stationsInLocation.Any(s =>
            s.StationName.Equals(CurrentStation.StationName, StringComparison.OrdinalIgnoreCase) &&
            s.Id != CurrentStation.Id);

        if (isDuplicate)
        {
            ShowToast = true;
            ToastMessage = "A station with this name already exists in this location.";
            ToastType = Toast.ToastType.Error;
            return false;
        }

        if (IsEditing)
        {
            await _stationService.UpdateStation(CurrentStation.Id, CurrentStation.StationName, CurrentStation.StationDescription, CurrentStation.IconId);
        }
        else
        {
            await _stationService.CreateStation(CurrentStation.LocationId, CurrentStation.StationName, CurrentStation.StationDescription, CurrentStation.IconId);
        }

        IsVisible = false;
        await _parentVM.LoadLocations();
        return true;
    }
}
