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

    [Inject]
    private ICartKeyService CartKeyService { get; set; } = default!;

    public bool IsInitialized { get; set; } = false;

    public int? CurrentLocationId { get; private set; }
    public int? PendingLocationId { get; private set; }
    public bool ShowConfirmModal { get; set; }

    public async Task HandleLocationSelected(int locationId)
    {
        var cartKey = await CartKeyService.GetCartKey();
        if (CurrentLocationId.HasValue && CurrentLocationId.Value != locationId)
        {
            PendingLocationId = locationId;
            ShowConfirmModal = true;
            return;
        }
        CurrentLocationId = locationId;
        var location = LocationSelectVM.Locations?.FirstOrDefault(l => l.Id == locationId);
        if (location == null) return;
        await CartService.SetLocation(cartKey, location);
        Navigation.NavigateTo("/station-select");
    }

    public async Task ConfirmLocationChange()
    {
        var cartKey = await CartKeyService.GetCartKey();
        if (!PendingLocationId.HasValue) return;
        var currentOrder = await CartService.GetOrder(cartKey);
        bool preservedIsCardOrder = currentOrder?.IsCardOrder ?? false;

        await CartService.ClearOrder(cartKey);
        var location = LocationSelectVM.Locations?.FirstOrDefault(l => l.Id == PendingLocationId.Value);
        if (location == null) return;

        CurrentLocationId = PendingLocationId.Value;
        PendingLocationId = null;
        await CartService.SetLocation(cartKey, location);
        await CartService.SetIsCardOrder(cartKey, preservedIsCardOrder);
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
                var cartKey = await CartKeyService.GetCartKey();
                var order = await CartService.GetOrder(cartKey);
                if (order?.Location != null)
                {
                    CurrentLocationId = order.Location.Id;
                }
                StateHasChanged();
            });
        }
    }
}