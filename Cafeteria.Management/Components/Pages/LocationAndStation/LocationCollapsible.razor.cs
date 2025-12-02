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

    [Parameter]
    public bool IsExpanded { get; set; }

    [Parameter]
    public EventCallback OnToggle { get; set; }

    public List<StationDto> Stations { get; set; } = [];

    protected override async Task OnInitializedAsync()
    {
        Stations = await StationService.GetStationsByLocation(Location.Id);
    }

    private async Task Toggle()
    {
        if (OnToggle.HasDelegate)
        {
            await OnToggle.InvokeAsync();
        }
    }
}
