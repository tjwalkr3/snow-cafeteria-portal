using Microsoft.AspNetCore.Components;
using Cafeteria.Shared.DTOs;
using Cafeteria.Management.Services;

namespace Cafeteria.Management.Components.Pages.LocationAndStation;

public partial class LocationCollapsible : ComponentBase
{
    [Inject]
    public IStationService StationService { get; set; } = default!;

    [Parameter, EditorRequired]
    public LocationDto Location { get; set; } = default!;

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    public List<StationDto> Stations { get; set; } = [];

    private bool IsCollapsed { get; set; } = true;

    protected override async Task OnInitializedAsync()
    {
        Stations = await StationService.GetStationsByLocation(Location.Id);
    }

    private void Toggle()
    {
        IsCollapsed = !IsCollapsed;
    }
}
