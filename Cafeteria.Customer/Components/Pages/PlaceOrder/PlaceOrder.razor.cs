using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.WebUtilities;
using Cafeteria.Customer.Services.Cart;
using Cafeteria.Shared.DTOs.Order;
using Cafeteria.Customer.Services.Printer;
using Cafeteria.Customer.Services.Order;
using Cafeteria.Customer.Services.Swipe;
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
    private IPrinterService PrinterService { get; set; } = default!;

    [Inject]
    private IApiOrderService OrderService { get; set; } = default!;

    [Inject]
    private IApiSwipeService SwipeService { get; set; } = default!;

    [Inject]
    private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;

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
    private List<EntreeGroup> EntreeGroups { get; set; } = new();
    private List<SideGroup> SideGroups { get; set; } = new();
    private List<DrinkGroup> DrinkGroups { get; set; } = new();

    private int AccountSwipeBalance { get; set; } = 0;

    public bool IsInitialized { get; set; } = false;

    public bool CanPlaceOrder => !Order?.IsCardOrder ?? false ? GetTotalSwipeCount() <= AccountSwipeBalance : true;

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

        return QueryHelpers.AddQueryString("/location-select", queryParameters);
    }

    private async Task HandlePlaceOrder()
    {
        if (Order == null)
        {
            _toastMessage = "No order to place!";
            _showToast = true;
            StateHasChanged();
            return;
        }

        try
        {
            var createOrderDto = PlaceOrderVM.ConvertToCreateOrderDto(Order);
            var createdOrder = await OrderService.CreateOrder(createOrderDto);

            var totalWithTax = Price + PlaceOrderVM.CalculateTax(Order);
            _toastMessage = Order.IsCardOrder
                ? $"Your order of ${totalWithTax:F2} has been placed successfully! Order #{createdOrder.Id}"
                : $"Your order has been placed successfully! Order #{createdOrder.Id}";
            _showToast = true;
            StateHasChanged();

            await Task.Delay(3000);
            await Cart.ClearOrder("order");

            if (Order?.Location != null)
            {
                await PrintPlacedOrder(Order.Location.Id, createdOrder.Id);
            }
        }
        catch (Exception ex)
        {
            _toastMessage = $"Failed to place order: {ex.Message}";
            _showToast = true;
            StateHasChanged();
            return;
        }

        Navigation.NavigateTo("/", true);
    }

    private async Task PrintPlacedOrder(int locationId, int orderId)
    {
        try
        {
            var printerUrl = await PrinterService.GetPrinterUrl(locationId);
            if (!string.IsNullOrWhiteSpace(printerUrl))
            {
                var printOrderData = new PrintOrderDto
                {
                    Id = orderId,
                    OrderTime = DateTime.Now,
                    FoodItems = ConvertOrderToFoodItems()
                };
                var printSuccess = await PrinterService.PrintOrder(printerUrl, printOrderData);
                if (!printSuccess)
                {
                    _toastMessage = "Order placed successfully, but receipt printing failed";
                    _showToast = true;
                    StateHasChanged();
                }
            }
        }
        catch (Exception ex)
        {
            _toastMessage = $"Order placed successfully, but receipt printing failed: {ex.Message}";
            _showToast = true;
            StateHasChanged();
        }
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

        return EntreeGroups.Sum(g => g.Quantity) + SideGroups.Sum(g => g.Quantity) + DrinkGroups.Sum(g => g.Quantity);
    }

    private decimal GetItemPrice(OrderEntreeItem item)
    {
        return item.Entree.EntreePrice + CalculateOptionsCost(item.SelectedOptions);
    }

    private decimal GetItemPrice(OrderSideItem item)
    {
        return item.Side.SidePrice + CalculateOptionsCost(item.SelectedOptions);
    }

    private decimal CalculateOptionsCost(List<SelectedFoodOption> selectedOptions)
    {
        if (selectedOptions == null || selectedOptions.Count == 0)
            return 0m;

        decimal cost = 0m;

        // Group options by their type ID
        var optionsByType = selectedOptions.GroupBy(opt => opt.OptionType.Id);

        foreach (var group in optionsByType)
        {
            var optionType = group.First().OptionType;
            var selectedCount = group.Count();
            var chargeableCount = Math.Max(0, selectedCount - optionType.NumIncluded);
            cost += chargeableCount * optionType.FoodOptionPrice;
        }

        return cost;
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

    private async Task AddEntreeItem(EntreeGroup group)
    {
        await Cart.AddEntree("order", group.Entree.Entree);
        foreach (var option in group.Entree.SelectedOptions)
        {
            await Cart.AddEntreeOption("order", group.Entree.Entree.Id, option.Option, option.OptionType);
        }
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
        await Cart.AddSide("order", group.Side.Side);
        foreach (var option in group.Side.SelectedOptions)
        {
            await Cart.AddSideOption("order", group.Side.Side.Id, option.Option, option.OptionType);
        }
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

    private List<FoodItemDto> ConvertOrderToFoodItems()
    {
        if (Order == null) return new List<FoodItemDto>();

        var foodItems = new List<FoodItemDto>();

        foreach (var entreeItem in Order.Entrees)
        {
            foodItems.Add(new FoodItemDto
            {
                Name = entreeItem.Entree.EntreeName,
                Options = entreeItem.SelectedOptions.Select(opt => new FoodItemOptionDto
                {
                    FoodOptionName = opt.Option.FoodOptionName
                }).ToList()
            });
        }

        foreach (var sideItem in Order.Sides)
        {
            foodItems.Add(new FoodItemDto
            {
                Name = sideItem.Side.SideName,
                Options = sideItem.SelectedOptions.Select(opt => new FoodItemOptionDto
                {
                    FoodOptionName = opt.Option.FoodOptionName
                }).ToList()
            });
        }

        foreach (var drink in Order.Drinks)
        {
            foodItems.Add(new FoodItemDto
            {
                Name = drink.DrinkName,
                Options = new List<FoodItemOptionDto>()
            });
        }

        return foodItems;
    }
}