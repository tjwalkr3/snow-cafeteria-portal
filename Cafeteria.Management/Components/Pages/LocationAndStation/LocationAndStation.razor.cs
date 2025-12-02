namespace Cafeteria.Management.Components.Pages.LocationAndStation;
using Microsoft.AspNetCore.Components;
using Cafeteria.Management.Services;

public partial class LocationAndStation : ComponentBase
{
    [Inject]
    public ILocationAndStationVM ViewModel { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
    }
}