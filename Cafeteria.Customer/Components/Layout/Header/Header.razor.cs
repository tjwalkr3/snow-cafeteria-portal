
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Cafeteria.Customer.Services;
using Cafeteria.Customer.Services.Cart;

namespace Cafeteria.Customer.Components.Layout.Header;

public partial class Header : ComponentBase, IDisposable
{
    [Inject]
    private ICartService CartService { get; set; } = default!;

    [Inject]
    private ICartKeyService CartKeyService { get; set; } = default!;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    [Inject]
    private CartNotificationService CartNotification { get; set; } = default!;

    private int cartItemCount = 0;
    private bool showUserMenu = false;

    private void ToggleUserMenu()
    {
        showUserMenu = !showUserMenu;
    }

    private void CloseUserMenu()
    {
        showUserMenu = false;
    }

    protected override async Task OnInitializedAsync()
    {
        await LoadCartCount();
        NavigationManager.LocationChanged += OnLocationChanged;
        CartNotification.OnCartChanged += OnCartChanged;
    }

    private async void OnLocationChanged(object? sender, LocationChangedEventArgs e)
    {
        showUserMenu = false;
        await LoadCartCount();
        await InvokeAsync(StateHasChanged);
    }

    private async void OnCartChanged()
    {
        await LoadCartCount();
        await InvokeAsync(StateHasChanged);
    }

    private async Task LoadCartCount()
    {
        var cartKey = await CartKeyService.GetCartKey();
        var order = await CartService.GetOrder(cartKey);
        if (order != null)
        {
            if (!order.IsCardOrder)
            {
                cartItemCount = Math.Min(order.Entrees.Count,
                Math.Min(order.Sides.Count, order.Drinks.Count));
            }
            else
            {
                cartItemCount = order.Entrees.Count + order.Sides.Count + order.Drinks.Count;
            }
        }
        else
        {
            cartItemCount = 0;
        }
    }

    private void NavigateToCart()
    {
        NavigationManager.NavigateTo("/place-order");
    }

    private void NavigateToOrderHistory()
    {
        NavigationManager.NavigateTo("/order-history");
    }

    private void SignOut()
    {
        NavigationManager.NavigateTo("/auth/signout", forceLoad: true);
    }

    public void Dispose()
    {
        NavigationManager.LocationChanged -= OnLocationChanged;
        CartNotification.OnCartChanged -= OnCartChanged;
    }
}