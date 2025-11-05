using Cafeteria.Shared.DTOs;

namespace Cafeteria.Customer.Components.Pages.Stations.DeliSwipe;

public interface IDeliSwipeVM
{
    List<SideDto> Sides { get; }
    List<DrinkDto> Drinks { get; }

    string ActiveTab { get; }
    SideDto? SelectedSide { get; }
    DrinkDto? SelectedDrink { get; }
    string? OrderConfirmation { get; }

    string? SelectedBread { get; }
    string? SelectedMeat { get; }
    string? SelectedCheese { get; }
    List<string> SelectedToppings { get; }
    string? SelectedDressing { get; }

    List<string> BreadOptions { get; }
    List<string> MeatOptions { get; }
    List<string> CheeseOptions { get; }
    List<string> ToppingOptions { get; }
    List<string> DressingOptions { get; }

    void SetActiveTab(string tab);
    void SelectSide(SideDto side);
    void SelectDrink(DrinkDto drink);
    void ToggleTopping(string topping);
    void SetBread(string bread);
    void SetMeat(string meat);
    void SetCheese(string cheese);
    void SetDressing(string dressing);

    int GetSelectionCount();
    string GetSelectionSummary();
    bool IsValidSelection();
    void AddToOrder();
    void ClearOrderConfirmation();
}
