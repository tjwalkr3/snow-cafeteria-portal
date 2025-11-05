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

    protected override async Task OnInitializedAsync()
    {
        string userName = "test";
        var entree = new EntreeDto { Id = 1, EntreeName = "Burger", EntreePrice = 5.00m };
        var side = new SideDto { Id = 1, SideName = "Fries", SidePrice = 3.00m};
        var drink = new DrinkDto { Id = 1, DrinkName = "Coke" };
        var option = new FoodOptionDto { Id = 1, FoodOptionName = "Tomato" };
        var optionType = new FoodOptionTypeDto { Id = 1, FoodOptionTypeName = "Toppings", FoodOptionPrice = 0.50m };

        await Cart.AddEntree(userName, entree);
        await Cart.AddEntreeOption(userName, entree.Id, option, optionType);
        await Cart.AddSide(userName, side);
        await Cart.AddDrink(userName, drink);

        Order = await GetOrder(userName);
    }
    
    private async Task<BrowserOrder?> GetOrder(string userName) 
    {
        return await Cart.GetOrder(userName);
    }
}