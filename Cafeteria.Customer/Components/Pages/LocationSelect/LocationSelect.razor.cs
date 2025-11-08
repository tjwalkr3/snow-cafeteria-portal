using Cafeteria.Customer.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;

namespace Cafeteria.Customer.Components.Pages.LocationSelect;

public partial class LocationSelect : ComponentBase
{
    [Inject]
    private NavigationManager Navigation { get; set; } = default!;

    [Inject]
    private ILocationSelectVM LocationSelectVM { get; set; } = default!;

    [Inject]
    private ICartService CartService { get; set; } = default!;

    [SupplyParameterFromQuery(Name = "payment")]
    public string? Payment { get; set; }
    public bool IsInitialized { get; set; } = false;

    public string CreateUrl(int locationId)
    {
        Dictionary<string, string?> queryParameters = new() { };

        if (!string.IsNullOrEmpty(Payment))
            queryParameters.Add("payment", Payment);
        queryParameters.Add("location", locationId.ToString());

        return QueryHelpers.AddQueryString("/station-select", queryParameters);
    }

    protected override async Task OnInitializedAsync()
    {
        LocationSelectVM.ValidatePaymentParameter(Payment);
        await LocationSelectVM.InitializeLocationsAsync();
        IsInitialized = true;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await CartService.ClearOrder("test");
            StateHasChanged();
        }
    }
}