using Cafeteria.Customer.Components.Pages.Stations.Configuration;
using Cafeteria.Customer.Components.Pages.Stations.Models;
using Cafeteria.Customer.Components.Pages.Stations.Strategies;
using Cafeteria.Customer.Services;
using Cafeteria.Customer.Services.Menu;
using Cafeteria.Shared.DTOs.Menu;

namespace Cafeteria.Customer.Components.Pages.Stations.GenericSwipe;

public class GenericSwipeVM : IGenericSwipeVM
{
    private readonly IApiMenuService _menuService;
    private readonly IStationConfigurationProvider _configProvider;
    private readonly ISelectionStrategyFactory _strategyFactory;
    private ISelectionStrategy? _strategy;

    public GenericSwipeVM(
        IApiMenuService menuService,
        IStationConfigurationProvider configProvider,
        ISelectionStrategyFactory strategyFactory)
    {
        _menuService = menuService;
        _configProvider = configProvider;
        _strategyFactory = strategyFactory;
    }

    // Data
    public List<EntreeDto> Entrees { get; private set; } = new();
    public List<SideDto> Sides { get; private set; } = new();
    public List<DrinkDto> Drinks { get; private set; } = new();
    public List<FoodOptionTypeWithOptionsDto> OptionTypes => _strategy?.GetOptionTypes() ?? new();
    public List<string> AvailableToppings => _strategy?.GetAvailableToppings() ?? new();

    // Configuration
    public StationConfiguration Configuration { get; private set; } = null!;
    public StationType CurrentStationType { get; private set; }

    // State
    public SelectionState State { get; } = new();
    public string ActiveTab { get; private set; } = "entrees";
    public bool IsCardOrder { get; private set; }
    public int StationId { get; private set; }
    public int LocationId { get; private set; }

    public async Task InitializeAsync(StationType stationType, int stationId, int locationId, bool isCardOrder)
    {
        // Clear any previous selections when switching stations
        State.Clear();

        CurrentStationType = stationType;
        Configuration = _configProvider.GetConfiguration(stationType);
        _strategy = _strategyFactory.CreateStrategy(stationType);

        if (_strategy is BaseSelectionStrategy baseStrategy)
        {
            baseStrategy.SetStationInfo(stationId, locationId);
        }

        StationId = stationId;
        LocationId = locationId;
        IsCardOrder = isCardOrder;

        // Set default active tab
        ActiveTab = Configuration.DefaultTab;

        // Load data
        Entrees = await _menuService.GetEntreesByStation(stationId);
        Sides = await _menuService.GetSidesByStation(stationId);
        Drinks = await _menuService.GetDrinksByLocation(locationId);

        // Let strategy perform any post-load setup
        await _strategy.OnDataLoadedAsync(Entrees, Sides, Drinks, State);
    }

    public void SetActiveTab(string tab)
    {
        ActiveTab = tab;
    }

    public async Task SelectEntreeAsync(EntreeDto entree)
    {
        if (_strategy != null)
        {
            await _strategy.OnEntreeSelectedAsync(entree, State);
        }
        else
        {
            State.SelectedEntree = entree;
        }
    }

    public void SelectSide(SideDto side)
    {
        State.SelectedSide = side;
    }

    public void SelectDrink(DrinkDto drink)
    {
        State.SelectedDrink = drink;
    }

    public void SetOptionForType(int optionTypeId, string optionName)
    {
        _strategy?.SetOptionForType(optionTypeId, optionName, State);
    }

    public void ToggleOptionForType(int optionTypeId, string optionName)
    {
        _strategy?.ToggleOptionForType(optionTypeId, optionName, State);
    }

    public void ToggleTopping(string topping)
    {
        _strategy?.ToggleTopping(topping, State);
    }

    public string? GetSelectedOption(int optionTypeId)
    {
        return State.SingleSelectOptions.TryGetValue(optionTypeId, out var value) ? value : null;
    }

    public List<string> GetSelectedOptionsForType(int optionTypeId)
    {
        return State.MultiSelectOptions.TryGetValue(optionTypeId, out var value) ? value : new List<string>();
    }

    public bool IsOptionSelected(int optionTypeId, string optionName)
    {
        // Check single-select options first
        if (State.SingleSelectOptions.TryGetValue(optionTypeId, out var singleValue))
        {
            return singleValue == optionName;
        }

        // Check multi-select options
        return GetSelectedOptionsForType(optionTypeId).Contains(optionName);
    }

    public bool IsValidSelection()
    {
        return _strategy?.IsValidSelection(State, IsCardOrder) ?? false;
    }

    public int GetSelectionCount()
    {
        return _strategy?.GetSelectionCount(State) ?? 0;
    }

    public string GetSelectionSummary()
    {
        return _strategy?.GetSelectionSummary(State) ?? "No items selected";
    }

    public decimal GetExtraToppingCharge()
    {
        return _strategy?.GetExtraToppingCharge(State) ?? 0m;
    }

    public bool HasExtraToppingCharge()
    {
        return _strategy?.HasExtraToppingCharge(State) ?? false;
    }

    public async Task<bool> AddToOrderAsync()
    {
        if (_strategy == null || !IsValidSelection())
            return false;

        await _strategy.AddToCartAsync(State, IsCardOrder);

        // Reset active tab to default
        ActiveTab = Configuration.DefaultTab;

        return true;
    }

    public void ClearSelections()
    {
        _strategy?.ClearSelections(State, Entrees);
        ActiveTab = Configuration.DefaultTab;
    }

    public bool IsMultiSelectOptionType(FoodOptionTypeWithOptionsDto optionType)
    {
        if (_strategy is DeliSelectionStrategy deliStrategy)
        {
            return deliStrategy.IsMultiSelectOptionType(optionType);
        }

        // For non-Deli stations, use configuration
        return Configuration.OptionSelectionMode == OptionSelectionMode.MultiSelect ||
               (Configuration.OptionSelectionMode == OptionSelectionMode.Mixed &&
                (optionType.OptionType.MaxAmount > 1 || optionType.OptionType.FoodOptionTypeName == "Toppings"));
    }
}
