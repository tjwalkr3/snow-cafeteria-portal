using Microsoft.AspNetCore.Components;
using Cafeteria.Shared.DTOs;
using Cafeteria.Management.Services;
using Cafeteria.Shared.Enums;

namespace Cafeteria.Management.Components.Pages.LocationAndStation;

public partial class LocationCollapsible : ComponentBase
{
    [Inject]
    public IStationService StationService { get; set; } = default!;

    [Inject]
    public ILocationService LocationService { get; set; } = default!;

    [Parameter, EditorRequired]
    public LocationDto Location { get; set; } = default!;

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public bool IsExpanded { get; set; }

    [Parameter]
    public EventCallback OnToggle { get; set; }

    public List<StationDto> Stations { get; set; } = [];
    public List<LocationBusinessHoursDto> LocationHours { get; set; } = [];
    public Dictionary<int, List<StationBusinessHoursDto>> StationHours { get; set; } = [];

    protected override async Task OnInitializedAsync()
    {
        Stations = await StationService.GetStationsByLocation(Location.Id);
        LocationHours = await LocationService.GetLocationBusinessHours(Location.Id);

        foreach (var station in Stations)
        {
            var hours = await StationService.GetStationBusinessHours(station.Id);
            StationHours[station.Id] = hours;
        }
    }

    private async Task Toggle()
    {
        if (OnToggle.HasDelegate)
        {
            await OnToggle.InvokeAsync();
        }
    }
}
