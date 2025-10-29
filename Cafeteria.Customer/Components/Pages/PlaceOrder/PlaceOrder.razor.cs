using Microsoft.AspNetCore.Components;

namespace Cafeteria.Customer.Components.Pages.PlaceOrder;

public partial class PlaceOrder : ComponentBase
{
    [Inject]
    private IPlaceOrderVM PlaceOrderVM { get; set; } = default!;

    [Inject]
    private NavigationManager Navigation { get; set; } = default!;

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