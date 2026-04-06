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
    private ICartKeyService CartKeyService { get; set; } = default!;

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
                var cartKey = await CartKeyService.GetCartKey();

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

                Order = await GetOrder(cartKey);

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
        var cartKey = await CartKeyService.GetCartKey();
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

        await Cart.ClearOrder(cartKey);

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
        var cartKey = await CartKeyService.GetCartKey();
        var entreeOptions = swipe.Entree.SelectedOptions.Select(o =>
            new SelectedFoodOption { Option = o.Option, OptionType = o.OptionType }).ToList();
        var sideOptions = swipe.Side?.SelectedOptions.Select(o =>
            new SelectedFoodOption { Option = o.Option, OptionType = o.OptionType }).ToList() ?? new List<SelectedFoodOption>();

        await Cart.AddEntreeWithOptions(cartKey, swipe.Entree.Entree, entreeOptions);
        if (swipe.Side != null)
        {
            await Cart.AddSideWithOptions(cartKey, swipe.Side.Side, sideOptions);
        }
        await Cart.AddDrink(cartKey, swipe.Drink);

        Order = await Cart.GetOrder(cartKey);
        if (Order != null)
        {
            SwipeGroups = PlaceOrderVM.GroupItemsIntoSwipes(Order);
        }
        CartNotification.NotifyCartChanged();
        StateHasChanged();
    }

    private async Task DecreaseSwipeQuantity(SwipeGroup swipe)
    {
        var cartKey = await CartKeyService.GetCartKey();
        await Cart.RemoveEntree(cartKey, swipe.Entree.Entree.Id);
        if (swipe.Side != null)
            await Cart.RemoveSide(cartKey, swipe.Side.Side.Id);
        await Cart.RemoveDrink(cartKey, swipe.Drink.Id);

        Order = await Cart.GetOrder(cartKey);
        if (Order != null)
        {
            SwipeGroups = PlaceOrderVM.GroupItemsIntoSwipes(Order);
        }
        CartNotification.NotifyCartChanged();
        StateHasChanged();
    }

    private async Task RemoveSwipeGroup(SwipeGroup swipe)
    {
        var cartKey = await CartKeyService.GetCartKey();
        for (int i = 0; i < swipe.Quantity; i++)
        {
            await Cart.RemoveEntree(cartKey, swipe.Entree.Entree.Id);
            if (swipe.Side != null)
                await Cart.RemoveSide(cartKey, swipe.Side.Side.Id);
            await Cart.RemoveDrink(cartKey, swipe.Drink.Id);
        }

        Order = await Cart.GetOrder(cartKey);
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
        var cartKey = await CartKeyService.GetCartKey();
        var options = group.Entree.SelectedOptions.Select(o =>
            new SelectedFoodOption { Option = o.Option, OptionType = o.OptionType }).ToList();
        await Cart.AddEntreeWithOptions(cartKey, group.Entree.Entree, options);
        await RefreshCardOrder();
    }

    private async Task RemoveEntreeItem(EntreeGroup group)
    {
        var cartKey = await CartKeyService.GetCartKey();
        if (Order == null) return;
        await Cart.RemoveEntree(cartKey, group.Entree.Entree.Id);
        await RefreshCardOrder();
    }

    private async Task RemoveAllEntreeItems(EntreeGroup group)
    {
        if (Order == null) return;
        var cartKey = await CartKeyService.GetCartKey();
        for (int i = 0; i < group.Quantity; i++)
        {
            await Cart.RemoveEntree(cartKey, group.Entree.Entree.Id);
        }
        await RefreshCardOrder();
    }

    private async Task AddSideItem(SideGroup group)
    {
        var cartKey = await CartKeyService.GetCartKey();
        var options = group.Side.SelectedOptions.Select(o =>
            new SelectedFoodOption { Option = o.Option, OptionType = o.OptionType }).ToList();
        await Cart.AddSideWithOptions(cartKey, group.Side.Side, options);
        await RefreshCardOrder();
    }

    private async Task RemoveSideItem(SideGroup group)
    {
        var cartKey = await CartKeyService.GetCartKey();
        if (Order == null) return;
        await Cart.RemoveSide(cartKey, group.Side.Side.Id);
        await RefreshCardOrder();
    }

    private async Task RemoveAllSideItems(SideGroup group)
    {
        if (Order == null) return;
        var cartKey = await CartKeyService.GetCartKey();
        for (int i = 0; i < group.Quantity; i++)
        {
            await Cart.RemoveSide(cartKey, group.Side.Side.Id);
        }
        await RefreshCardOrder();
    }

    private async Task AddDrinkItem(DrinkGroup group)
    {
        var cartKey = await CartKeyService.GetCartKey();
        await Cart.AddDrink(cartKey, group.Drink);
        await RefreshCardOrder();
    }

    private async Task RemoveDrinkItem(DrinkGroup group)
    {
        var cartKey = await CartKeyService.GetCartKey();
        if (Order == null) return;
        await Cart.RemoveDrink(cartKey, group.Drink.Id);
        await RefreshCardOrder();
    }

    private async Task RemoveAllDrinkItems(DrinkGroup group)
    {
        if (Order == null) return;
        var cartKey = await CartKeyService.GetCartKey();
        for (int i = 0; i < group.Quantity; i++)
        {
            await Cart.RemoveDrink(cartKey, group.Drink.Id);
        }
        await RefreshCardOrder();
    }

    private async Task RefreshCardOrder()
    {
        var cartKey = await CartKeyService.GetCartKey();
        Order = await Cart.GetOrder(cartKey);
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