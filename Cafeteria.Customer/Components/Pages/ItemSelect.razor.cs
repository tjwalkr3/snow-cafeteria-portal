using Microsoft.AspNetCore.Components;
using Cafeteria.Shared.DTOs;

namespace Cafeteria.Customer.Components.Pages;

public partial class ItemSelect
{
    public bool IsInitialized { get; set; } = false;
    public List<FoodItemDto> foodItems = new();

    protected override async Task OnInitializedAsync()
    {
        await ItemSelectVM.GetDataFromRouteParameters(this.Navigation.Uri);
        foodItems = ItemSelectVM.GetFoodItems();
        IsInitialized = true;
    }

    private string GetFoodItemUrl(string foodItemId)
    {
        return $"/food-item?item-id={Uri.EscapeDataString(foodItemId)}"; // TODO: this line has a magic string in it for the route parameter of the food-item page that we should perhaps make into a variable somewhere...maybe in an enum?
    }
}