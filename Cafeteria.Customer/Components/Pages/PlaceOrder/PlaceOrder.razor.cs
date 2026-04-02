using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Cafeteria.Customer.Services.Cart;
using Cafeteria.Shared.DTOs.Order;
using Cafeteria.Shared.DTOs.Menu;
using Cafeteria.Customer.Services.Order;
using Cafeteria.Customer.Services.Swipe;
using Cafeteria.Shared.Utilities;
using System.Security.Claims;

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

    [Inject]
    private IApiOrderService OrderService { get; set; } = default!;

    [Inject]
    private IApiSwipeService SwipeService { get; set; } = default!;

    [Inject]
    private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;

    private BrowserOrder? Order { get; set; } = null;

    private decimal Price { get; set; } = 0.0m;

    private bool _isLoading = true;
    private List<SwipeGroup> SwipeGroups { get; set; } = new();
    private List<EntreeGroup> EntreeGroups { get; set; } = new();
    private List<SideGroup> SideGroups { get; set; } = new();
    private List<DrinkGroup> DrinkGroups { get; set; } = new();

    private int AccountSwipeBalance { get; set; } = 0;
    private bool _hasAttemptedPlaceOrder;

    public bool IsInitialized { get; set; } = false;

    public bool CanPlaceOrder => !Order?.IsCardOrder ?? false ? GetTotalSwipeCount() <= AccountSwipeBalance : true;

    protected override Task OnInitializedAsync()
    {
        IsInitialized = true;
        return Task.CompletedTask;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await InvokeAsync(async () =>
            {
                string userName = "order";

                var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
                var user = authState.User;

                if (user?.Identity?.IsAuthenticated ?? false)
                {
                    var email = user.FindFirst(ClaimTypes.Email)?.Value ?? user.FindFirst("email")?.Value;
                    if (!string.IsNullOrEmpty(email))
                    {
                        try
                        {
                            var swipeData = await SwipeService.GetSwipesByEmail(email);
                            if (swipeData != null)
                            {
                                AccountSwipeBalance = swipeData.SwipeBalance;
                            }
                        }
                        catch (Exception)
                        {
                            AccountSwipeBalance = 0;
                        }
                    }
                }

                Order = await GetOrder(userName);

                if (Order != null)
                {
                    Price = PlaceOrderVM.CalculateTotalPrice(Order);

                    if (!Order.IsCardOrder)
                    {
                        SwipeGroups = PlaceOrderVM.GroupItemsIntoSwipes(Order);
                    }
                    else
                    {
                        EntreeGroups = PlaceOrderVM.GroupEntrees(Order);
                        SideGroups = PlaceOrderVM.GroupSides(Order);
                        DrinkGroups = PlaceOrderVM.GroupDrinks(Order);
                    }
                }

                CartNotification.NotifyCartChanged();

                _isLoading = false;
                StateHasChanged();
            });
        }
    }

    private async Task<BrowserOrder?> GetOrder(string userName)
    {
        return await Cart.GetOrder(userName);
    }

    public string GetStationSelectUrl() => "/location-select";

    private async Task HandlePlaceOrder()
    {
        if (_hasAttemptedPlaceOrder)
            return;

        if (Order == null)
        {
            return;
        }

        _hasAttemptedPlaceOrder = true;
        StateHasChanged();

        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        Order.UserName = GetDisplayName(user);

        await OrderService.CreateOrder(Order);

        await Cart.ClearOrder("order");

        Navigation.NavigateTo("/thank-you", true);
    }

    private static string GetDisplayName(ClaimsPrincipal user)
    {
        var explicitName = user.FindFirst("name")?.Value;
        if (!string.IsNullOrWhiteSpace(explicitName))
            return explicitName;

        var givenName = user.FindFirst("given_name")?.Value;
        var familyName = user.FindFirst("family_name")?.Value;
        var fullName = $"{givenName} {familyName}".Trim();
        if (!string.IsNullOrWhiteSpace(fullName))
            return fullName;

        var preferredUserName = user.FindFirst("preferred_username")?.Value;
        if (!string.IsNullOrWhiteSpace(preferredUserName))
            return preferredUserName;

        var email = user.FindFirst(ClaimTypes.Email)?.Value ?? user.FindFirst("email")?.Value;
        if (!string.IsNullOrWhiteSpace(email))
            return email;

        return "Unknown User";
    }

    private int GetTotalItemCount()
    {
        if (Order == null) return 0;

        if (!Order.IsCardOrder)
        {
            return GetTotalSwipeCount();
        }

        return EntreeGroups.Sum(g => g.Quantity) + SideGroups.Sum(g => g.Quantity) + DrinkGroups.Sum(g => g.Quantity);
    }

    private decimal GetItemPrice(OrderEntreeItem item) =>
        item.Entree.EntreePrice + OrderCalculations.CalculateOptionsCost(item.SelectedOptions);

    private decimal GetItemPrice(OrderSideItem item) =>
        item.Side.SidePrice + OrderCalculations.CalculateOptionsCost(item.SelectedOptions);

    private async Task IncreaseSwipeQuantity(SwipeGroup swipe)
    {
        var entreeOptions = swipe.Entree.SelectedOptions.Select(o =>
            new SelectedFoodOption { Option = o.Option, OptionType = o.OptionType }).ToList();
        var sideOptions = swipe.Side.SelectedOptions.Select(o =>
            new SelectedFoodOption { Option = o.Option, OptionType = o.OptionType }).ToList();

        await Cart.AddEntreeWithOptions("order", swipe.Entree.Entree, entreeOptions);
        await Cart.AddSideWithOptions("order", swipe.Side.Side, sideOptions);
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
        if (swipe.Side != null)
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
            if (swipe.Side != null)
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

    private async Task AddEntreeItem(EntreeGroup group)
    {
        var options = group.Entree.SelectedOptions.Select(o =>
            new SelectedFoodOption { Option = o.Option, OptionType = o.OptionType }).ToList();
        await Cart.AddEntreeWithOptions("order", group.Entree.Entree, options);
        await RefreshCardOrder();
    }

    private async Task RemoveEntreeItem(EntreeGroup group)
    {
        if (Order == null) return;
        await Cart.RemoveEntree("order", group.Entree.Entree.Id);
        await RefreshCardOrder();
    }

    private async Task RemoveAllEntreeItems(EntreeGroup group)
    {
        if (Order == null) return;
        for (int i = 0; i < group.Quantity; i++)
        {
            await Cart.RemoveEntree("order", group.Entree.Entree.Id);
        }
        await RefreshCardOrder();
    }

    private async Task AddSideItem(SideGroup group)
    {
        var options = group.Side.SelectedOptions.Select(o =>
            new SelectedFoodOption { Option = o.Option, OptionType = o.OptionType }).ToList();
        await Cart.AddSideWithOptions("order", group.Side.Side, options);
        await RefreshCardOrder();
    }

    private async Task RemoveSideItem(SideGroup group)
    {
        if (Order == null) return;
        await Cart.RemoveSide("order", group.Side.Side.Id);
        await RefreshCardOrder();
    }

    private async Task RemoveAllSideItems(SideGroup group)
    {
        if (Order == null) return;
        for (int i = 0; i < group.Quantity; i++)
        {
            await Cart.RemoveSide("order", group.Side.Side.Id);
        }
        await RefreshCardOrder();
    }

    private async Task AddDrinkItem(DrinkGroup group)
    {
        await Cart.AddDrink("order", group.Drink);
        await RefreshCardOrder();
    }

    private async Task RemoveDrinkItem(DrinkGroup group)
    {
        if (Order == null) return;
        await Cart.RemoveDrink("order", group.Drink.Id);
        await RefreshCardOrder();
    }

    private async Task RemoveAllDrinkItems(DrinkGroup group)
    {
        if (Order == null) return;
        for (int i = 0; i < group.Quantity; i++)
        {
            await Cart.RemoveDrink("order", group.Drink.Id);
        }
        await RefreshCardOrder();
    }

    private async Task RefreshCardOrder()
    {
        Order = await Cart.GetOrder("order");
        if (Order != null)
        {
            Price = PlaceOrderVM.CalculateTotalPrice(Order);
            if (Order.IsCardOrder)
            {
                EntreeGroups = PlaceOrderVM.GroupEntrees(Order);
                SideGroups = PlaceOrderVM.GroupSides(Order);
                DrinkGroups = PlaceOrderVM.GroupDrinks(Order);
            }
        }
        CartNotification.NotifyCartChanged();
        StateHasChanged();
    }

}