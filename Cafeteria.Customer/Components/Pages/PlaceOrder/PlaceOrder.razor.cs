using Microsoft.AspNetCore.Components;
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
    private IApiMenuService MenuService { get; set; } = default!;

    [SupplyParameterFromQuery(Name = "location")]
    public int Location { get; set; }

    [SupplyParameterFromQuery(Name = "payment")]
    public string? Payment { get; set; }

    private BrowserOrder? Order { get; set; } = null;

    private decimal Price { get; set; } = 0.0m;

    private bool _isLoading = true;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            string userName = "test";

            // If payment and location are provided in the URL, persist them to the cart (localStorage)
            await SavePaymentMethod(userName);
            await SaveLocation(userName);

            // Get the cart from local storage and calculate the cost
            Order = await GetOrder(userName);
            
            if (Order == null || (Order.Entrees.Count == 0 && Order.Sides.Count == 0 && Order.Drinks.Count == 0))
            {
                await SetSampleData(userName);
                Order = await GetOrder(userName);
            }
            
            if (Order != null) Price = PlaceOrderVM.CalculateTotalPrice(Order);

            _isLoading = false;
            StateHasChanged();
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
            try
            {
                var locations = await MenuService.GetAllLocations();
                var locationDto = locations.FirstOrDefault(l => l.Id == Location);
                if (locationDto != null)
                {
                    await Cart.SetLocation(userName, locationDto);
                }
            }
            catch
            {
                // ignore failures here; the page will continue and show missing data if needed
            }
        }
    }

    private async Task<BrowserOrder?> GetOrder(string userName)
    {
        return await Cart.GetOrder(userName);
    }

    private async Task SetSampleData(string userName) 
    {
        var location = new LocationDto { Id = 1, LocationName = "Badger Den", LocationDescription = "The main cafeteria in the student center." };
        var entree = new EntreeDto { Id = 1, EntreeName = "Burger", EntreePrice = 5.00m };
        var side = new SideDto { Id = 1, SideName = "Fries", SidePrice = 3.00m };
        var drink = new DrinkDto { Id = 1, DrinkName = "Coke", DrinkPrice = 2.00m };
        var option = new FoodOptionDto { Id = 1, FoodOptionName = "Tomato" };
        var optionType = new FoodOptionTypeDto { Id = 1, FoodOptionTypeName = "Toppings", FoodOptionPrice = 0.50m };

        await Cart.SetIsCardOrder(userName, false);
        await Cart.SetLocation(userName, location);
        await Cart.AddEntree(userName, entree);
        await Cart.AddEntreeOption(userName, entree.Id, option, optionType);
        await Cart.AddSide(userName, side);
        await Cart.AddDrink(userName, drink);
    }
}