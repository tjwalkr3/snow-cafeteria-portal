using Microsoft.AspNetCore.Components;
using Cafeteria.Shared.DTOs.Menu;
using Cafeteria.Management.Services.Locations;
using Cafeteria.Management.Services.Stations;

namespace Cafeteria.Management.Components.Pages.LocationAndStation;

public partial class LocationAndStation : ComponentBase
{
    [Inject]
    public ILocationService LocationService { get; set; } = default!;

    [Inject]
    public IStationService StationService { get; set; } = default!;

    private List<LocationDto> Locations { get; set; } = [];
    private List<StationDto> Stations { get; set; } = [];
    private int? SelectedLocationId { get; set; }
    private LocationDto? SelectedLocation => Locations.FirstOrDefault(l => l.Id == SelectedLocationId);
    private string ActiveTab { get; set; } = "hours";

    protected override async Task OnInitializedAsync()
    {
        Locations = await LocationService.GetAllLocations();
        if (Locations.Any())
        {
            await SelectLocation(Locations.First().Id);
        }
    }

    private async Task SelectLocation(int locationId)
    {
        SelectedLocationId = locationId;
        Stations = await StationService.GetStationsByLocation(locationId);
        ActiveTab = "hours";
    }

    private void SetActiveTab(string tabName)
    {
        ActiveTab = tabName;
    }
}
