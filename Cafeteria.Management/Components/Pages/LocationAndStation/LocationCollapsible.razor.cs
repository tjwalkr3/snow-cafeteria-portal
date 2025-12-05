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

    [Parameter]
    public EventCallback OnEdit { get; set; }

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

    private async Task Edit()
    {
        if (OnEdit.HasDelegate)
        {
            await OnEdit.InvokeAsync();
        }
    }

    private bool AreHoursDifferent(List<LocationBusinessHoursDto> locationHours, List<StationBusinessHoursDto> stationHours)
    {
        if (locationHours.Count != stationHours.Count)
        {
            return true;
        }

        var sortedLoc = locationHours.OrderBy(h => h.WeekdayId).ThenBy(h => h.OpenTime).ToList();
        var sortedStation = stationHours.OrderBy(h => h.WeekdayId).ThenBy(h => h.OpenTime).ToList();

        for (int i = 0; i < sortedLoc.Count; i++)
        {
            if (sortedLoc[i].WeekdayId != sortedStation[i].WeekdayId ||
                sortedLoc[i].OpenTime != sortedStation[i].OpenTime ||
                sortedLoc[i].CloseTime != sortedStation[i].CloseTime)
            {
                return true;
            }
        }

        return false;
    }
}
