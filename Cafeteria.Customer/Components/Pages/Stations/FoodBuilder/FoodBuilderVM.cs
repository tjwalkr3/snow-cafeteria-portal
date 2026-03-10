using Cafeteria.Customer.Components.Pages.Stations.Domain;
using Cafeteria.Customer.Services.Menu;
using Cafeteria.Shared.DTOs.Menu;

namespace Cafeteria.Customer.Components.Pages.Stations.FoodBuilder;

public class FoodBuilderVM : IFoodBuilderVM
{
    private readonly IApiMenuService _menuService;
    private readonly CartSubmitter _cartSubmitter;

    public FoodBuilderVM(IApiMenuService menuService, CartSubmitter cartSubmitter)
    {
        _menuService = menuService;
        _cartSubmitter = cartSubmitter;
    }

    public List<EntreeDto> Entrees { get; private set; } = new();
    public List<SideWithOptionsDto> Sides { get; private set; } = new();
    public List<DrinkDto> Drinks { get; private set; } = new();
    public List<FoodOptionTypeWithOptionsDto> OptionTypes { get; private set; } = new();
    public List<TabDefinition> Tabs { get; private set; } = new();
    public string PageTitle { get; private set; } = string.Empty;

    public SelectionState State { get; } = new();
    public string ActiveTab { get; private set; } = "entrees";
    public bool IsCardOrder { get; private set; }
    public int StationId { get; private set; }
    public int LocationId { get; private set; }

    public async Task InitializeAsync(int stationId, int locationId, bool isCardOrder, string stationName)
    {
        State.Clear();
        StationId = stationId;
        LocationId = locationId;
        IsCardOrder = isCardOrder;
        PageTitle = string.IsNullOrEmpty(stationName) ? "Station" : stationName;

        Entrees = await _menuService.GetEntreesByStation(stationId);
        Sides = await _menuService.GetSidesWithOptionsByStation(stationId);
        Drinks = await _menuService.GetDrinksByLocation(locationId);
        OptionTypes = new List<FoodOptionTypeWithOptionsDto>();

        Tabs = new List<TabDefinition>
        {
            new("entrees", "Entrees", isDefault: true),
            new("sides", "Sides"),
            new("drinks", "Drinks")
        };
        ActiveTab = "entrees";
    }

    public void SetActiveTab(string tab) => ActiveTab = tab;

    public async Task SelectEntreeAsync(EntreeDto entree)
    {
        State.SelectedEntree = entree;
        State.ClearOptionsOnly();
        OptionTypes = await _menuService.GetOptionTypesWithOptionsByEntree(entree.Id);
    }

    public void SelectSide(SideDto side) => State.SelectedSide = side;

    public void SelectDrink(DrinkDto drink) => State.SelectedDrink = drink;

    public bool IsValidSelection() =>
        SelectionValidator.IsValid(State, OptionTypes, IsCardOrder, requiresOptionsComplete: OptionTypes.Any());

    public async Task<bool> AddToOrderAsync()
    {
        if (!IsValidSelection())
            return false;

        var sideWithOptions = Sides.FirstOrDefault(s => s.Side.Id == State.SelectedSide?.Id);
        await _cartSubmitter.SubmitAsync(State, OptionTypes, new List<FoodOptionDto>(), sideWithOptions?.OptionTypes);
        State.Clear();
        ActiveTab = Tabs.FirstOrDefault()?.Id ?? "entrees";
        return true;
    }

    public void ClearSelections()
    {
        State.Clear();
        ActiveTab = Tabs.FirstOrDefault()?.Id ?? "entrees";
    }
}
