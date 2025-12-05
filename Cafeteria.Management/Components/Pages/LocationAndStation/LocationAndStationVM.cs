using Cafeteria.Management.Components.Pages.LocationAndStation.Location;
using Cafeteria.Management.Services;
using Cafeteria.Shared.DTOs;

namespace Cafeteria.Management.Components.Pages.LocationAndStation;

public class LocationAndStationVM : ILocationAndStationVM, IDisposable
{
    private readonly ILocationService _locationService;
    private readonly ICreateOrEditLocationVM _createOrEditLocationViewModel;

    public LocationAndStationVM(ILocationService locationService, ICreateOrEditLocationVM createOrEditLocationViewModel)
    {
        _locationService = locationService;
        _createOrEditLocationViewModel = createOrEditLocationViewModel;
        _createOrEditLocationViewModel.OnLocationSaved += HandleLocationSaved;
    }

    public List<LocationDto> Locations { get; private set; } = [];
    public int? ExpandedLocationId { get; private set; }
    public event Action? OnStateChanged;

    public async Task LoadStationsAsync()
    {
        Locations = await _locationService.GetAllLocations();
        OnStateChanged?.Invoke();
    }

    private async Task HandleLocationSaved()
    {
        await LoadStationsAsync();
    }

    public void OpenCreateLocationModal()
    {
        _createOrEditLocationViewModel.SelectedLocation = new LocationDto
        {
            LocationName = "",
            LocationDescription = ""
        };
        _createOrEditLocationViewModel.Show();
    }

    public void EditLocation(LocationDto location)
    {
        _createOrEditLocationViewModel.SelectedLocation = new LocationDto
        {
            Id = location.Id,
            LocationName = location.LocationName,
            LocationDescription = location.LocationDescription,
            ImageUrl = location.ImageUrl
        };
        _createOrEditLocationViewModel.Show();
    }

    public void ToggleLocation(int locationId)
    {
        if (ExpandedLocationId == locationId)
        {
            ExpandedLocationId = null;
        }
        else
        {
            ExpandedLocationId = locationId;
        }
        OnStateChanged?.Invoke();
    }

    public void Dispose()
    {
        _createOrEditLocationViewModel.OnLocationSaved -= HandleLocationSaved;
    }
}