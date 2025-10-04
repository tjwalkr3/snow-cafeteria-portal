using Microsoft.AspNetCore.Components;

namespace Cafeteria.Customer.Components.Pages;

public partial class StationSelect
{
    public bool IsInitialized { get; set; } = false;

    protected override async Task OnInitializedAsync()
    {
        await StationSelectVM.GetDataFromRouteParameters(this.Navigation.Uri);
        IsInitialized = true;
    }
}