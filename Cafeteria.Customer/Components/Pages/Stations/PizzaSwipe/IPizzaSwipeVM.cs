using Cafeteria.Shared.DTOs;

namespace Cafeteria.Customer.Components.Pages.Stations.PizzaSwipe;

public interface IPizzaSwipeVM
{
    List<EntreeDto> Entrees { get; }
    List<DrinkDto> Drinks { get; }

    string ActiveTab { get; }
    EntreeDto? SelectedEntree { get; }
    DrinkDto? SelectedDrink { get; }
    List<string> SelectedToppings { get; }
    string? OrderConfirmation { get; }

    List<string> AvailableToppings { get; }

    void SetActiveTab(string tab);
    void SelectEntree(EntreeDto entree);
    void SelectDrink(DrinkDto drink);
    void ToggleTopping(string topping);

    int GetSelectionCount();
    bool IsValidSelection();
    void AddToOrder();
    void ClearOrderConfirmation();
}
