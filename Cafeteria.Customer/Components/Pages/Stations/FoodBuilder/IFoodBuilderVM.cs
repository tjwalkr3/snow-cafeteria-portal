using Cafeteria.Customer.Components.Pages.Stations.Configuration;
using Cafeteria.Customer.Components.Pages.Stations.Models;
using Cafeteria.Shared.DTOs.Menu;

namespace Cafeteria.Customer.Components.Pages.Stations.FoodBuilder;

public interface IFoodBuilderVM
{
    List<EntreeDto> Entrees { get; }
    List<SideDto> Sides { get; }
    List<DrinkDto> Drinks { get; }
    List<FoodOptionTypeWithOptionsDto> OptionTypes { get; }
    List<string> AvailableToppings { get; }

    StationConfiguration Configuration { get; }
    StationType CurrentStationType { get; }

    SelectionState State { get; }
    string ActiveTab { get; }
    bool IsCardOrder { get; }
    int StationId { get; }
    int LocationId { get; }

    Task InitializeAsync(StationType stationType, int stationId, int locationId, bool isCardOrder);

    void SetActiveTab(string tab);

    Task SelectEntreeAsync(EntreeDto entree);
    void SelectSide(SideDto side);
    void SelectDrink(DrinkDto drink);

    void SetOptionForType(int optionTypeId, string optionName);
    void ToggleOptionForType(int optionTypeId, string optionName);
    void ToggleTopping(string topping);
    string? GetSelectedOption(int optionTypeId);
    List<string> GetSelectedOptionsForType(int optionTypeId);
    bool IsOptionSelected(int optionTypeId, string optionName);

    bool IsValidSelection();
    int GetSelectionCount();
    string GetSelectionSummary();
    decimal GetExtraToppingCharge();
    bool HasExtraToppingCharge();

    Task<bool> AddToOrderAsync();
    void ClearSelections();

    bool IsMultiSelectOptionType(FoodOptionTypeWithOptionsDto optionType);
}
