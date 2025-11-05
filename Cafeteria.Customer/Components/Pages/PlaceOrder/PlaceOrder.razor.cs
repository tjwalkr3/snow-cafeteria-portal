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

    private BrowserOrder? Order { get; set; } = null;

    private decimal Price { get; set; } = 0.0m;

    private bool _isLoading = true;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            // Test data that can be removed after the page before this is complete
            string userName = "test";
            if (await GetOrder(userName) == null)
            {
                var entree = new EntreeDto { Id = 1, EntreeName = "Burger", EntreePrice = 5.00m };
                var side = new SideDto { Id = 1, SideName = "Fries", SidePrice = 3.00m };
                var drink = new DrinkDto { Id = 1, DrinkName = "Coke", DrinkPrice = 2.00m };
                var option = new FoodOptionDto { Id = 1, FoodOptionName = "Tomato" };
                var optionType = new FoodOptionTypeDto { Id = 1, FoodOptionTypeName = "Toppings", FoodOptionPrice = 0.50m };

                await Cart.AddEntree(userName, entree);
                await Cart.AddEntreeOption(userName, entree.Id, option, optionType);
                await Cart.AddSide(userName, side);
                await Cart.AddDrink(userName, drink);
            }

            // The code that gets the cart from local storage and calculates the cost
            Order = await GetOrder(userName);
            if (Order != null) Price = PlaceOrderVM.CalculateTotalPrice(Order);

            _isLoading = false;
            StateHasChanged();
        }
    }

    private async Task<BrowserOrder?> GetOrder(string userName)
    {
        return await Cart.GetOrder(userName);
    }
}