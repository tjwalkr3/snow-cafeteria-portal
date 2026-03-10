using Cafeteria.Customer.Components.Pages.Stations.Domain;
using Cafeteria.Shared.DTOs.Menu;

namespace Cafeteria.Customer.Components.Pages.Stations.FoodBuilder;

public interface IFoodBuilderVM
{
    List<EntreeDto> Entrees { get; }
    List<SideWithOptionsDto> Sides { get; }
    List<DrinkDto> Drinks { get; }
    List<FoodOptionTypeWithOptionsDto> OptionTypes { get; }
    List<TabDefinition> Tabs { get; }
    string PageTitle { get; }

    SelectionState State { get; }
    string ActiveTab { get; }
    bool IsCardOrder { get; }
    int StationId { get; }
    int LocationId { get; }

    Task InitializeAsync(int stationId, int locationId, bool isCardOrder, string stationName);

    void SetActiveTab(string tab);

    Task SelectEntreeAsync(EntreeDto entree);
    void SelectSide(SideDto side);
    void SelectDrink(DrinkDto drink);

    bool IsValidSelection();

    Task<bool> AddToOrderAsync();
    void ClearSelections();
}

