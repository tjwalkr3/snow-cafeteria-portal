using Cafeteria.Shared.DTOs;

namespace Cafeteria.Customer.Components.Pages;

public partial class PlaceOrder
{
    private List<FoodItemDto> foodItems = new();

    protected override void OnInitialized()
    {
        foodItems = PlaceOrderVM.GetOrderItems();
    }
}