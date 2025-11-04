using Cafeteria.Shared.DTOs;

namespace Cafeteria.Customer.Components.Pages.Stations.BreakfastSwipe;

public interface IBreakfastSwipeVM
{
    List<EntreeDto> Entrees { get; }
    List<SideDto> Sides { get; }
    List<DrinkDto> Drinks { get; }

    string ActiveTab { get; }
    EntreeDto? SelectedEntree { get; }
    SideDto? SelectedSide { get; }
    DrinkDto? SelectedDrink { get; }
    string? OrderConfirmation { get; }

    string? SelectedMeatOption { get; }
    string? SelectedBreadOption { get; }

    void SetActiveTab(string tab);
    void SelectEntree(EntreeDto entree);
    void SelectSide(SideDto side);
    void SelectDrink(DrinkDto drink);
    void SetMeatOption(string meat);
    void SetBreadOption(string bread);

    bool RequiresMeatSelection(int entreeId);
    bool RequiresBreadSelection(int entreeId);
    int GetSelectionCount();
    bool IsValidSelection();
    void AddToOrder();
    void ClearOrderConfirmation();
}
