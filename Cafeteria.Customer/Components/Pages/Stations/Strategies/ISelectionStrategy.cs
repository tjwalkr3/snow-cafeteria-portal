using Cafeteria.Customer.Components.Pages.Stations.Configuration;
using Cafeteria.Customer.Components.Pages.Stations.Domain;
using Cafeteria.Shared.DTOs.Menu;

namespace Cafeteria.Customer.Components.Pages.Stations.Strategies;

public interface ISelectionStrategy
{
    StationType StationType { get; }

    bool IsValidSelection(SelectionState state, bool isCardOrder);

    int GetSelectionCount(SelectionState state);

    string GetSelectionSummary(SelectionState state);

    Task AddToCartAsync(SelectionState state, bool isCardOrder);

    Task OnDataLoadedAsync(
        List<EntreeDto> entrees,
        List<SideDto> sides,
        List<DrinkDto> drinks,
        SelectionState state);

    void ClearSelections(SelectionState state, List<EntreeDto> entrees);

    // Option handling
    Task OnEntreeSelectedAsync(EntreeDto entree, SelectionState state);

    void SetOptionForType(int optionTypeId, string optionName, SelectionState state);

    void ToggleOptionForType(int optionTypeId, string optionName, SelectionState state);

    void ToggleTopping(string topping, SelectionState state);

    // For strategies that need to expose option types
    List<FoodOptionTypeWithOptionsDto> GetOptionTypes();

    List<string> GetAvailableToppings();

    // For price calculations
    decimal GetExtraToppingCharge(SelectionState state);

    bool HasExtraToppingCharge(SelectionState state);
}
