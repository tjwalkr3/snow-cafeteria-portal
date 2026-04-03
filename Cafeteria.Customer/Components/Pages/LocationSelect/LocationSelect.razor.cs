using Cafeteria.Customer.Services;
using Cafeteria.Customer.Services.Cart;
using Microsoft.AspNetCore.Components;

namespace Cafeteria.Customer.Components.Pages.LocationSelect;

public partial class LocationSelect : ComponentBase
{
    [Inject]
    private NavigationManager Navigation { get; set; } = default!;

    [Inject]
    private ILocationSelectVM LocationSelectVM { get; set; } = default!;

    [Inject]
    private ICartService CartService { get; set; } = default!;

    public bool IsInitialized { get; set; } = false;

    public int? CurrentLocationId { get; private set; }
    public int? PendingLocationId { get; private set; }
    public bool ShowConfirmModal { get; set; }

    public async Task HandleLocationSelected(int locationId)
    {
        if (CurrentLocationId.HasValue && CurrentLocationId.Value != locationId)
        {
            PendingLocationId = locationId;
            ShowConfirmModal = true;
            return;
        }
        CurrentLocationId = locationId;
        var location = LocationSelectVM.Locations?.FirstOrDefault(l => l.Id == locationId);
        if (location == null) return;
        await CartService.SetLocation("order", location);
        Navigation.NavigateTo("/station-select");
    }

    public async Task ConfirmLocationChange()
    {
        if (!PendingLocationId.HasValue) return;
        // Preserve the payment method when changing locations
        var currentOrder = await CartService.GetOrder("order");
        bool preservedIsCardOrder = currentOrder?.IsCardOrder ?? false;

        await CartService.ClearOrder("order");
        var location = LocationSelectVM.Locations?.FirstOrDefault(l => l.Id == PendingLocationId.Value);
        if (location == null) return;

        CurrentLocationId = PendingLocationId.Value;
        PendingLocationId = null;
        await CartService.SetLocation("order", location);
        // Restore the payment method
        await CartService.SetIsCardOrder("order", preservedIsCardOrder);
        Navigation.NavigateTo("/station-select");
    }

    public void CancelLocationChange()
    {
        PendingLocationId = null;
        ShowConfirmModal = false;
    }

    protected override async Task OnInitializedAsync()
    {
        await LocationSelectVM.InitializeLocationsAsync();
        IsInitialized = true;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await InvokeAsync(async () =>
            {
                var order = await CartService.GetOrder("order");
                if (order?.Location != null)
                {
                    CurrentLocationId = order.Location.Id;
                }
                StateHasChanged();
            });
        }
    }
}