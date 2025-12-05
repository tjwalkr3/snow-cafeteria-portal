namespace Cafeteria.Management.Components.Pages.LocationAndStation;
using Microsoft.AspNetCore.Components;
using Cafeteria.Management.Services;
using Cafeteria.Management.Components.Pages.LocationAndStation.Location;
using Cafeteria.Shared.DTOs;

public partial class LocationAndStation : ComponentBase, IDisposable
{
    [Inject]
    public ILocationAndStationVM ViewModel { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        ViewModel.OnStateChanged += StateHasChanged;
        await ViewModel.LoadStationsAsync();
        await base.OnInitializedAsync();
    }

    public void Dispose()
    {
        ViewModel.OnStateChanged -= StateHasChanged;
    }
}