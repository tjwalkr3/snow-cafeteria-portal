using Cafeteria.Shared.DTOs;

namespace Cafeteria.Customer.Components.Pages.Stations.PizzaSwipe;

public interface IPizzaSwipeVM
{
    List<EntreeDto> Entrees { get; }
    List<DrinkDto> Drinks { get; }
    List<FoodOptionDto> AllEntreeOptions { get; }

    string ActiveTab { get; }
    EntreeDto? SelectedEntree { get; }
    DrinkDto? SelectedDrink { get; }
    List<string> SelectedToppings { get; }

    List<string> AvailableToppings { get; }

    int StationId { get; set; }
    int LocationId { get; set; }

    Task LoadDataAsync(int stationId, int locationId);
    void SetActiveTab(string tab);
    void SelectEntree(EntreeDto entree);
    void SelectDrink(DrinkDto drink);
    void ToggleTopping(string topping);

    int GetSelectionCount();
    bool IsValidSelection();
    Task<bool> AddToOrderAsync();
}
