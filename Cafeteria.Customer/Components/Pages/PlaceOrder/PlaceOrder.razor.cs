using Microsoft.AspNetCore.Components;

namespace Cafeteria.Customer.Components.Pages.PlaceOrder;

public partial class PlaceOrder
{
    protected override async Task OnInitializedAsync()
    {
        await PlaceOrderVM.GetDataFromRouteParameters(this.Navigation.Uri);
    }

    private decimal GetTotalPrice()
    {
        if (PlaceOrderVM.SelectedFoodItem == null)
            return 0m;

        return PlaceOrderVM.SelectedFoodItem.ItemPrice + PlaceOrderVM.SelectedIngredients.Sum(i => i.IngredientPrice ?? 0);
    }
}