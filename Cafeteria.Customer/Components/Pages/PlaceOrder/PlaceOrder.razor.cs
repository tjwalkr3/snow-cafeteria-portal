using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Cafeteria.Customer.Services;
using Cafeteria.Shared.DTOs;

namespace Cafeteria.Customer.Components.Pages.PlaceOrder;

public partial class PlaceOrder : ComponentBase
{
    [Inject]
    private IPlaceOrderVM PlaceOrderVM { get; set; } = default!;

    [Inject]
    private NavigationManager Navigation { get; set; } = default!;

    [Inject]
    private ICartService Cart { get; set; } = default!;

    [Inject]
    private CartNotificationService CartNotification { get; set; } = default!;

    [SupplyParameterFromQuery(Name = "location")]
    public int Location { get; set; }

    [SupplyParameterFromQuery(Name = "payment")]
    public string? Payment { get; set; }

    private BrowserOrder? Order { get; set; } = null;

    private decimal Price { get; set; } = 0.0m;

    private bool _isLoading = true;
    private bool _showToast = false;
    private string _toastMessage = "";
    private List<SwipeGroup> SwipeGroups { get; set; } = new();

    public bool IsInitialized { get; set; } = false;

    protected override async Task OnInitializedAsync()
    {
        await PlaceOrderVM.InitializeLocations();
        IsInitialized = true;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await InvokeAsync(async () =>
            {
                string userName = "order";

                await SavePaymentMethod(userName);
                await SaveLocation(userName);

                Order = await GetOrder(userName);

                if (Order != null)
                {
                    Price = PlaceOrderVM.CalculateTotalPrice(Order);

                    if (!Order.IsCardOrder)
                    {
                        SwipeGroups = PlaceOrderVM.GroupItemsIntoSwipes(Order);
                    }
                }

                CartNotification.NotifyCartChanged();

                _isLoading = false;
                StateHasChanged();
            });
        }
    }

    private async Task SavePaymentMethod(string userName)
    {
        if (!string.IsNullOrEmpty(Payment))
        {
            await Cart.SetIsCardOrder(userName, Payment == "card");
        }
    }

    private async Task SaveLocation(string userName)
    {
        if (Location != 0)
        {
            var locationDto = PlaceOrderVM.GetLocationById(Location);
            if (locationDto != null)
            {
                await Cart.SetLocation(userName, locationDto);
            }
        }
    }

    private async Task<BrowserOrder?> GetOrder(string userName)
    {
        return await Cart.GetOrder(userName);
    }

    public string GetStationSelectUrl()
    {
        Dictionary<string, string?> queryParameters = new() { };

        string? payment = null;
        if (Order != null)
        {
            payment = Order.IsCardOrder ? "card" : "swipe";
        }
        else if (!string.IsNullOrEmpty(Payment))
        {
            payment = Payment;
        }

        if (!string.IsNullOrEmpty(payment))
        {
            queryParameters.Add("payment", payment);
        }

        int? locationId = null;
        if (Order?.Location != null)
        {
            locationId = Order.Location.Id;
        }
        else if (Location != 0)
        {
            locationId = Location;
        }

        if (locationId.HasValue && locationId.Value > 0)
        {
            queryParameters.Add("location", locationId.Value.ToString());
        }

        return QueryHelpers.AddQueryString("/station-select", queryParameters);
    }

    private async Task HandlePlaceOrder()
    {
        _toastMessage = Order?.IsCardOrder == true
            ? $"Your order of ${Price:F2} has been placed successfully!"
            : "Your order has been placed successfully!";
        _showToast = true;
        StateHasChanged();

        await Task.Delay(3000);

        await Cart.ClearOrder("order");

        Navigation.NavigateTo("/", true);
    }

    private void OnToastHidden()
    {
        _showToast = false;
        StateHasChanged();
    }

    private int GetTotalItemCount()
    {
        if (Order == null) return 0;

        if (!Order.IsCardOrder)
        {
            return GetTotalSwipeCount();
        }

        return Order.Entrees.Count + Order.Sides.Count + Order.Drinks.Count;
    }

    private decimal GetItemPrice(OrderEntreeItem item)
    {
        return item.Entree.EntreePrice + item.SelectedOptions.Sum(o => o.OptionType.FoodOptionPrice);
    }

    private decimal GetItemPrice(OrderSideItem item)
    {
        return item.Side.SidePrice + item.SelectedOptions.Sum(o => o.OptionType.FoodOptionPrice);
    }

    private async Task IncreaseSwipeQuantity(SwipeGroup swipe)
    {
        await Cart.AddEntree("order", swipe.Entree.Entree);
        foreach (var option in swipe.Entree.SelectedOptions)
        {
            await Cart.AddEntreeOption("order", swipe.Entree.Entree.Id,
                option.Option, option.OptionType);
        }

        await Cart.AddSide("order", swipe.Side.Side);
        foreach (var option in swipe.Side.SelectedOptions)
        {
            await Cart.AddSideOption("order", swipe.Side.Side.Id,
                option.Option, option.OptionType);
        }

        await Cart.AddDrink("order", swipe.Drink);

        Order = await Cart.GetOrder("order");
        if (Order != null)
        {
            SwipeGroups = PlaceOrderVM.GroupItemsIntoSwipes(Order);
        }
        CartNotification.NotifyCartChanged();
        StateHasChanged();
    }

    private async Task DecreaseSwipeQuantity(SwipeGroup swipe)
    {
        await Cart.RemoveEntree("order", swipe.Entree.Entree.Id);
        await Cart.RemoveSide("order", swipe.Side.Side.Id);
        await Cart.RemoveDrink("order", swipe.Drink.Id);

        Order = await Cart.GetOrder("order");
        if (Order != null)
        {
            SwipeGroups = PlaceOrderVM.GroupItemsIntoSwipes(Order);
        }
        CartNotification.NotifyCartChanged();
        StateHasChanged();
    }

    private async Task RemoveSwipeGroup(SwipeGroup swipe)
    {
        for (int i = 0; i < swipe.Quantity; i++)
        {
            await Cart.RemoveEntree("order", swipe.Entree.Entree.Id);
            await Cart.RemoveSide("order", swipe.Side.Side.Id);
            await Cart.RemoveDrink("order", swipe.Drink.Id);
        }

        Order = await Cart.GetOrder("order");
        if (Order != null)
        {
            SwipeGroups = PlaceOrderVM.GroupItemsIntoSwipes(Order);
        }
        CartNotification.NotifyCartChanged();
        StateHasChanged();
    }

    private int GetTotalSwipeCount()
    {
        return SwipeGroups.Sum(s => s.Quantity);
    }
}