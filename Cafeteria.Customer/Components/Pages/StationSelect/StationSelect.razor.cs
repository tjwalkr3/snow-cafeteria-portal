using Microsoft.AspNetCore.Components;

namespace Cafeteria.Customer.Components.Pages.StationSelect;

public partial class StationSelect : ComponentBase
{
    [Inject]
    private IStationSelectVM StationSelectVM { get; set; } = default!;

    [Inject]
    private NavigationManager Navigation { get; set; } = default!;

    public bool IsInitialized { get; set; } = false;

    protected override async Task OnInitializedAsync()
    {
        await StationSelectVM.GetDataFromRouteParameters(this.Navigation.Uri);
        IsInitialized = true;
    }
}