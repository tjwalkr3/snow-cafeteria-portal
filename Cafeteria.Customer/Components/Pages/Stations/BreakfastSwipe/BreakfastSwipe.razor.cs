using Microsoft.AspNetCore.Components;

namespace Cafeteria.Customer.Components.Pages.Stations.BreakfastSwipe;

public partial class BreakfastSwipe : ComponentBase
{
    [Inject]
    private IBreakfastSwipeVM VM { get; set; } = default!;

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

    private void SelectSide(Cafeteria.Shared.DTOs.SideDto side)
    {
        VM.SelectSide(side);
        StateHasChanged();
    }

    private void SelectDrink(Cafeteria.Shared.DTOs.DrinkDto drink)
    {
        VM.SelectDrink(drink);
        StateHasChanged();
    }

    private void SetMeatOption(string meat)
    {
        VM.SetMeatOption(meat);
        StateHasChanged();
    }

    private void SetBreadOption(string bread)
    {
        VM.SetBreadOption(bread);
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
