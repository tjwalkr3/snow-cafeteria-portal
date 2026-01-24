using Cafeteria.Customer.Components.Pages.Stations.Configuration;
using Cafeteria.Customer.Components.Pages.Stations.Models;
using Cafeteria.Shared.DTOs;

namespace Cafeteria.Customer.Components.Pages.Stations.GenericSwipe;

public interface IGenericSwipeVM
{
    // Data
    List<EntreeDto> Entrees { get; }
    List<SideDto> Sides { get; }
    List<DrinkDto> Drinks { get; }
    List<FoodOptionTypeWithOptionsDto> OptionTypes { get; }
    List<string> AvailableToppings { get; }

    // Configuration
    StationConfiguration Configuration { get; }
    StationType CurrentStationType { get; }

    // State
    SelectionState State { get; }
    string ActiveTab { get; }
    bool IsCardOrder { get; }
    int StationId { get; }
    int LocationId { get; }

    // Initialization
    Task InitializeAsync(StationType stationType, int stationId, int locationId, bool isCardOrder);

    // Tab navigation
    void SetActiveTab(string tab);

    // Selection methods
    Task SelectEntreeAsync(EntreeDto entree);
    void SelectSide(SideDto side);
    void SelectDrink(DrinkDto drink);

    // Option handling
    void SetOptionForType(int optionTypeId, string optionName);
    void ToggleOptionForType(int optionTypeId, string optionName);
    void ToggleTopping(string topping);
    string? GetSelectedOption(int optionTypeId);
    List<string> GetSelectedOptionsForType(int optionTypeId);
    bool IsOptionSelected(int optionTypeId, string optionName);

    // Validation and summary
    bool IsValidSelection();
    int GetSelectionCount();
    string GetSelectionSummary();
    decimal GetExtraToppingCharge();
    bool HasExtraToppingCharge();

    // Actions
    Task<bool> AddToOrderAsync();
    void ClearSelections();

    // Helpers
    bool IsMultiSelectOptionType(FoodOptionTypeWithOptionsDto optionType);
}
