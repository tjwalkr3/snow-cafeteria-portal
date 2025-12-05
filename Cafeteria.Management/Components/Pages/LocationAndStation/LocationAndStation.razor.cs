namespace Cafeteria.Management.Components.Pages.LocationAndStation;
using Microsoft.AspNetCore.Components;
using Cafeteria.Management.Services;
using Cafeteria.Management.Components.Pages.LocationAndStation.Location;
using Cafeteria.Shared.DTOs;

public partial class LocationAndStation : ComponentBase
{
    [Inject]
    public ILocationAndStationVM ViewModel { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        await ViewModel.LoadStationsAsync();
        await base.OnInitializedAsync();
    }
}