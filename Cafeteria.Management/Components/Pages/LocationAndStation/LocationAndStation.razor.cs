namespace Cafeteria.Management.Components.Pages.LocationAndStation;
using Microsoft.AspNetCore.Components;
using Cafeteria.Management.Services;
using Cafeteria.Management.Components.Pages.LocationAndStation.Location;
using Cafeteria.Shared.DTOs;

public partial class LocationAndStation : ComponentBase
{
    [Inject]
    public ILocationAndStationVM ViewModel { get; set; } = null!;

    private CreateOrEditLocation _createOrEditLocationModal = null!;
    private LocationDto _selectedLocation = new();

    private void OpenCreateLocationModal()
    {
        _selectedLocation = new LocationDto
        {
            LocationName = "",
            LocationDescription = ""
        };
        _createOrEditLocationModal.Show();
    }

    private int? ExpandedLocationId { get; set; }

    private void ToggleLocation(int locationId)
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

    protected override async Task OnInitializedAsync()
    {
        await ViewModel.LoadStationsAsync();
        await base.OnInitializedAsync();
    }
}