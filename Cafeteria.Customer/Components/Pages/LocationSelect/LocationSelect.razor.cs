using Microsoft.AspNetCore.Components;
using Cafeteria.Shared.DTOsOld;

namespace Cafeteria.Customer.Components.Pages.LocationSelect;

public partial class LocationSelect
{
    public bool IsInitialized { get; set; } = false;

    protected override async Task OnInitializedAsync()
    {
        await LocationSelectVM.InitializeLocationsAsync();
        IsInitialized = true;
    }
}