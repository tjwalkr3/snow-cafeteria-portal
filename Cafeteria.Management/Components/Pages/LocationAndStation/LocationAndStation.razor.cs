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

    private SchedulingExceptionsEditor? LocationExceptionsEditor { get; set; }
    private Dictionary<int, SchedulingExceptionsEditor?> StationExceptionsEditorRefs { get; set; } = new();
    private SchedulingExceptionsEditor? ActiveModalEditor { get; set; }

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
        StationExceptionsEditorRefs.Clear();
        foreach (var station in Stations)
        {
            StationExceptionsEditorRefs[station.Id] = null;
        }
    }

    private void SetActiveTab(string tabName)
    {
        ActiveTab = tabName;
    }

    private void OnExceptionsModalStateChanged()
    {
        // Find which editor has ShowModal = true
        if (LocationExceptionsEditor?.ShowModal == true)
        {
            ActiveModalEditor = LocationExceptionsEditor;
        }
        else
        {
            var activeStation = StationExceptionsEditorRefs.Values.FirstOrDefault(e => e?.ShowModal == true);
            ActiveModalEditor = activeStation;
        }

        StateHasChanged();
    }

    private SchedulingExceptionsEditor? GetActiveExceptionsEditor()
    {
        return ActiveModalEditor;
    }
}
