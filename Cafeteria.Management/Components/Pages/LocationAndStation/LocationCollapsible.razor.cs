using Microsoft.AspNetCore.Components;
using Cafeteria.Shared.DTOs;

namespace Cafeteria.Management.Components.Pages.LocationAndStation;

public partial class LocationCollapsible : ComponentBase
{
    [Parameter, EditorRequired]
    public LocationDto Location { get; set; } = default!;

    [Parameter]
    public RenderFragment? StationContainer { get; set; }

    private bool IsCollapsed { get; set; } = true;

    private void Toggle()
    {
        IsCollapsed = !IsCollapsed;
    }
}
