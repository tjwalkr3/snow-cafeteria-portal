using Microsoft.AspNetCore.Components;

namespace Cafeteria.Customer.Components.Pages.StationSelect;

public partial class StationSelect : ComponentBase
{
    [Inject]
    private IStationSelectVM StationSelectVM { get; set; } = default!;

    [Inject]
    private NavigationManager Navigation { get; set; } = default!;

    [SupplyParameterFromQuery(Name = "location")]
    public int Location { get; set; }

    [SupplyParameterFromQuery(Name = "payment")]
    public string? Payment { get; set; }

    public bool IsInitialized { get; set; } = false;

    protected override async Task OnInitializedAsync()
    {
        StationSelectVM.ValidateLocationParameter(Location, Payment);
        await StationSelectVM.InitializeStations(Location);
        IsInitialized = true;
    }
}