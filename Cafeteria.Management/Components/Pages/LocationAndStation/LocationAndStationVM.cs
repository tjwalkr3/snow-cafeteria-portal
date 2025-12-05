using Cafeteria.Management.Components.Pages.LocationAndStation.Location;
using Cafeteria.Management.Services;
using Cafeteria.Shared.DTOs;

namespace Cafeteria.Management.Components.Pages.LocationAndStation;

public class LocationAndStationVM(ILocationService locationService, ICreateOrEditLocationVM createOrEditLocationViewModel) : ILocationAndStationVM
{
    public List<LocationDto> Locations { get; private set; } = [];
    public int? ExpandedLocationId { get; private set; }

    public async Task LoadStationsAsync()
    {
        Locations = await locationService.GetAllLocations();
    }

    public void OpenCreateLocationModal()
    {
        createOrEditLocationViewModel.SelectedLocation = new LocationDto
        {
            LocationName = "",
            LocationDescription = ""
        };
        createOrEditLocationViewModel.Show();
    }

    public void EditLocation(LocationDto location)
    {
        createOrEditLocationViewModel.SelectedLocation = location;
        createOrEditLocationViewModel.Show();
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
    }
}