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

    protected override async Task OnInitializedAsync()
    {
        var entree = new EntreeDto { Id = 1, EntreeName = "Burger" };
        var side = new SideDto { Id = 1, SideName = "Fries" };
        var drink = new DrinkDto { Id = 1, DrinkName = "Coke" };
        var option = new FoodOptionDto { Id = 1, FoodOptionName = "Cheese" };
        var optionType = new FoodOptionTypeDto { Id = 1, FoodOptionTypeName = "Toppings" };
        
        await Cart.AddEntree("test", entree);
        await Cart.AddEntreeOption("test", entree.Id, option, optionType);
        await Cart.AddSide("test", side);
        await Cart.AddDrink("test", drink);
    }

    private decimal GetTotalPrice()
    {
        return 0;
    }
}