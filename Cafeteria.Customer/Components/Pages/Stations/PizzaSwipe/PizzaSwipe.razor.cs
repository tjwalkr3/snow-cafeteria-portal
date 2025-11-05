using Microsoft.AspNetCore.Components;

namespace Cafeteria.Customer.Components.Pages.Stations.PizzaSwipe;

public partial class PizzaSwipe : ComponentBase
{
    [Inject]
    private IPizzaSwipeVM VM { get; set; } = default!;

    private void SetActiveTab(string tab)
    {
        VM.SetActiveTab(tab);
        StateHasChanged();
    }

    private void SelectEntree(Cafeteria.Shared.DTOs.EntreeDto entree)
    {
        VM.SelectEntree(entree);
        StateHasChanged();
    }

    private void SelectDrink(Cafeteria.Shared.DTOs.DrinkDto drink)
    {
        VM.SelectDrink(drink);
        StateHasChanged();
    }

    private void ToggleTopping(string topping)
    {
        VM.ToggleTopping(topping);
        StateHasChanged();
    }

    private void AddToOrder()
    {
        VM.AddToOrder();
        StateHasChanged();
    }

    private void ClearOrderConfirmation()
    {
        VM.ClearOrderConfirmation();
        StateHasChanged();
    }
}
