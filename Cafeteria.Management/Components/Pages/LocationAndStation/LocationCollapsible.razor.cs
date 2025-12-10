using Microsoft.AspNetCore.Components;
using Cafeteria.Shared.DTOs;
using Cafeteria.Management.Services;
using Cafeteria.Shared.Enums;
using Cafeteria.Management.Components.Pages.LocationAndStation.Station;

namespace Cafeteria.Management.Components.Pages.LocationAndStation;

public partial class LocationCollapsible : ComponentBase, IDisposable
{
    [Inject]
    public IStationService StationService { get; set; } = default!;

    [Inject]
    public ILocationService LocationService { get; set; } = default!;

    [Inject]
    public ICreateOrEditStationVM CreateOrEditStationViewModel { get; set; } = default!;

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

    [Parameter]
    public EventCallback OnDelete { get; set; }

    public List<StationDto> Stations { get; set; } = [];
    public List<LocationBusinessHoursDto> LocationHours { get; set; } = [];
    public Dictionary<int, List<StationBusinessHoursDto>> StationHours { get; set; } = [];

    protected override async Task OnInitializedAsync()
    {
        CreateOrEditStationViewModel.OnStationSaved += HandleStationSaved;
        await LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        Stations = await StationService.GetStationsByLocation(Location.Id);
        LocationHours = await LocationService.GetLocationBusinessHours(Location.Id);

        foreach (var station in Stations)
        {
            var hours = await StationService.GetStationBusinessHours(station.Id);
            StationHours[station.Id] = hours;
        }
        StateHasChanged();
    }

    private async Task HandleStationSaved()
    {
        await LoadDataAsync();
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

    private async Task Delete()
    {
        if (OnDelete.HasDelegate)
        {
            await OnDelete.InvokeAsync();
        }
    }

    private void AddStation()
    {
        CreateOrEditStationViewModel.SelectedStation = new StationDto
        {
            StationName = "",
            StationDescription = ""
        };
        CreateOrEditStationViewModel.Show(Location.Id);
    }

    private void EditStation(StationDto station)
    {
        CreateOrEditStationViewModel.SelectedStation = new StationDto
        {
            Id = station.Id,
            StationName = station.StationName,
            StationDescription = station.StationDescription,
            LocationId = station.LocationId
        };
        CreateOrEditStationViewModel.Show(Location.Id);
    }

    private async Task DeleteStation(int stationId)
    {
        await StationService.DeleteStation(stationId);
        await LoadDataAsync();
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

    public void Dispose()
    {
        CreateOrEditStationViewModel.OnStationSaved -= HandleStationSaved;
    }
}
