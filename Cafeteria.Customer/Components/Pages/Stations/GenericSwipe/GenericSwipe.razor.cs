using Cafeteria.Customer.Components.Pages.Stations.Configuration;
using Cafeteria.Customer.Services.Cart;
using Cafeteria.Shared.DTOs.Menu;
using Microsoft.AspNetCore.Components;

namespace Cafeteria.Customer.Components.Pages.Stations.GenericSwipe;

public partial class GenericSwipe : ComponentBase
{
    [Inject]
    private IGenericSwipeVM VM { get; set; } = default!;

    [Inject]
    private IStationConfigurationProvider ConfigProvider { get; set; } = default!;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    [Inject]
    private ICartService Cart { get; set; } = default!;

    [Parameter]
    public string? StationType { get; set; }

    private bool _isLoading = true;
    private bool _showOptionsModal;
    private bool _showSandwichBuilderModal;
    private Dictionary<int, HashSet<string>> _stagedSandwichSelections = new();
    private bool _showWrapBuilderModal;
    private Dictionary<int, HashSet<string>> _stagedWrapSelections = new();
    private bool _showDeliOptionsModal;
    private int _activeDeliOptionTypeId;
    private HashSet<string> _stagedDeliSelections = new();
    private bool _showPizzaToppingsModal;
    private HashSet<string> _stagedToppings = new();
    private Dictionary<int, string> _stagedBreakfastOptions = new();
    private EntreeDto? _stagedEntree;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            var stationType = DetermineStationType();

            var order = await Cart.GetOrder("order");
            int stationId = order?.StationId ?? 0;
            int locationId = order?.Location?.Id ?? 0;
            bool isCardOrder = order?.IsCardOrder ?? false;

            await VM.InitializeAsync(stationType, stationId, locationId, isCardOrder);
            _isLoading = false;
            StateHasChanged();
        }
    }

    private Configuration.StationType DetermineStationType()
    {
        if (!string.IsNullOrEmpty(StationType) && ConfigProvider.TryParseStationType(StationType, out var parsedType))
        {
            return parsedType;
        }

        var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
        var path = uri.AbsolutePath.ToLowerInvariant().TrimStart('/');

        return path switch
        {
            "breakfast" => Configuration.StationType.Breakfast,
            "deli" => Configuration.StationType.Deli,
            "grill" => Configuration.StationType.Grill,
            "pizza" => Configuration.StationType.Pizza,
            "wrap" => Configuration.StationType.Wraps,
            _ => Configuration.StationType.Grill
        };
    }

    private string GetStationIcon()
    {
        return VM.CurrentStationType switch
        {
            Configuration.StationType.Grill => "bi-fire",
            Configuration.StationType.Breakfast => "bi-egg-fried",
            Configuration.StationType.Pizza => "bi-basket-fill",
            Configuration.StationType.Deli => "bi-cup-straw",
            Configuration.StationType.Wraps => "bi-tornado",
            _ => "bi-shop"
        };
    }

    private string GetCategoryIcon(string categoryName)
    {
        return categoryName.ToLower() switch
        {
            "bread" => "bi-slash-square",
            "meat" => "bi-egg-fill",
            "meat choice" => "bi-egg-fill",
            "cheese" => "bi-square-fill",
            "toppings" => "bi-leaf",
            "dressing" => "bi-droplet-fill",
            "plate base" => "bi-basket2-fill",
            "side" or "sides" => "bi-basket2-fill",
            "tortilla" => "bi-circle",
            "protein" => "bi-egg-fried",
            "sauce" => "bi-droplet-fill",
            _ => "bi-circle"
        };
    }

    public string CreateBackUrl() => "/station-select";

    private void SetActiveTab(string tab)
    {
        VM.SetActiveTab(tab);
        StateHasChanged();
    }

    private async Task SelectEntree(EntreeDto entree)
    {
        await VM.SelectEntreeAsync(entree);

        if (VM.Configuration?.EntreeSelectionLoadsOptions == true && VM.OptionTypes.Any())
        {
            _stagedEntree = entree;
            VM.State.SelectedEntree = null;
            _stagedBreakfastOptions = new Dictionary<int, string>(VM.State.SingleSelectOptions);
            VM.State.SingleSelectOptions.Clear();
            _showOptionsModal = true;
        }

        if (VM.CurrentStationType == Configuration.StationType.Pizza)
        {
            _stagedEntree = entree;
            VM.State.SelectedEntree = null;
            _stagedToppings = new HashSet<string>(VM.State.SelectedToppings);
            VM.State.SelectedToppings.Clear();
            _showPizzaToppingsModal = true;
        }

        StateHasChanged();
    }

    private void CloseOptionsModal()
    {
        // Discard the staged entree — user closed without clicking "Done"
        _stagedEntree = null;
        _stagedBreakfastOptions.Clear();
        _showOptionsModal = false;
        StateHasChanged();
    }

    private void OpenSandwichBuilderModal()
    {
        _stagedSandwichSelections = new Dictionary<int, HashSet<string>>();
        foreach (var optionTypeWithOptions in VM.OptionTypes)
        {
            var id = optionTypeWithOptions.OptionType.Id;
            _stagedSandwichSelections[id] = new HashSet<string>(VM.GetSelectedOptionsForType(id));
        }
        _showSandwichBuilderModal = true;
    }

    private void CloseSandwichBuilderModal()
    {
        _showSandwichBuilderModal = false;
    }

    private void ToggleStagedSandwichOption(int optionTypeId, string name)
    {
        if (!_stagedSandwichSelections.ContainsKey(optionTypeId))
            _stagedSandwichSelections[optionTypeId] = new HashSet<string>();

        if (!_stagedSandwichSelections[optionTypeId].Remove(name))
        {
            if (!VM.IsCardOrder)
            {
                var optionType = VM.OptionTypes.FirstOrDefault(o => o.OptionType.Id == optionTypeId);
                if (optionType != null && _stagedSandwichSelections[optionTypeId].Count >= optionType.OptionType.NumIncluded)
                    return;
            }
            _stagedSandwichSelections[optionTypeId].Add(name);
        }
        StateHasChanged();
    }

    private void SetStagedSandwichOption(int optionTypeId, string name)
    {
        _stagedSandwichSelections[optionTypeId] = new HashSet<string> { name };
        StateHasChanged();
    }

    private void ConfirmSandwichBuilder()
    {
        foreach (var optionTypeWithOptions in VM.OptionTypes)
        {
            var optTypeId = optionTypeWithOptions.OptionType.Id;
            var isMulti = VM.IsMultiSelectOptionType(optionTypeWithOptions);
            var currentSelections = VM.GetSelectedOptionsForType(optTypeId);
            var staged = _stagedSandwichSelections.GetValueOrDefault(optTypeId) ?? new HashSet<string>();

            if (isMulti)
            {
                var toRemove = currentSelections.Except(staged).ToList();
                var toAdd = staged.Except(currentSelections).ToList();
                foreach (var opt in toRemove)
                    VM.ToggleOptionForType(optTypeId, opt);
                foreach (var opt in toAdd)
                    VM.ToggleOptionForType(optTypeId, opt);
            }
            else
            {
                var selected = staged.FirstOrDefault();
                if (selected != null)
                    VM.SetOptionForType(optTypeId, selected);
            }
        }

        _showSandwichBuilderModal = false;
        StateHasChanged();
    }

    private void OpenWrapBuilderModal()
    {
        _stagedWrapSelections = new Dictionary<int, HashSet<string>>();
        foreach (var optionTypeWithOptions in VM.OptionTypes)
        {
            var id = optionTypeWithOptions.OptionType.Id;
            _stagedWrapSelections[id] = new HashSet<string>(VM.GetSelectedOptionsForType(id));
        }
        _showWrapBuilderModal = true;
    }

    private void CloseWrapBuilderModal()
    {
        _showWrapBuilderModal = false;
    }

    private void ToggleStagedWrapOption(int optionTypeId, string name)
    {
        if (!_stagedWrapSelections.ContainsKey(optionTypeId))
            _stagedWrapSelections[optionTypeId] = new HashSet<string>();

        if (!_stagedWrapSelections[optionTypeId].Remove(name))
        {
            if (!VM.IsCardOrder)
            {
                var optionType = VM.OptionTypes.FirstOrDefault(o => o.OptionType.Id == optionTypeId);
                if (optionType != null && _stagedWrapSelections[optionTypeId].Count >= optionType.OptionType.NumIncluded)
                    return;
            }
            _stagedWrapSelections[optionTypeId].Add(name);
        }
        StateHasChanged();
    }

    private void SetStagedWrapOption(int optionTypeId, string name)
    {
        _stagedWrapSelections[optionTypeId] = new HashSet<string> { name };
        StateHasChanged();
    }

    private void ConfirmWrapBuilder()
    {
        foreach (var optionTypeWithOptions in VM.OptionTypes)
        {
            var optTypeId = optionTypeWithOptions.OptionType.Id;
            var isMulti = VM.IsMultiSelectOptionType(optionTypeWithOptions);
            var currentSelections = VM.GetSelectedOptionsForType(optTypeId);
            var staged = _stagedWrapSelections.GetValueOrDefault(optTypeId) ?? new HashSet<string>();

            if (isMulti)
            {
                var toRemove = currentSelections.Except(staged).ToList();
                var toAdd = staged.Except(currentSelections).ToList();
                foreach (var opt in toRemove)
                    VM.ToggleOptionForType(optTypeId, opt);
                foreach (var opt in toAdd)
                    VM.ToggleOptionForType(optTypeId, opt);
            }
            else
            {
                var selected = staged.FirstOrDefault();
                if (selected != null)
                    VM.SetOptionForType(optTypeId, selected);
            }
        }

        _showWrapBuilderModal = false;
        StateHasChanged();
    }

    private void OpenDeliOptionsModal(int optionTypeId)
    {
        _activeDeliOptionTypeId = optionTypeId;
        _stagedDeliSelections = new HashSet<string>(VM.GetSelectedOptionsForType(optionTypeId));
        _showDeliOptionsModal = true;
    }

    private void CloseDeliOptionsModal()
    {
        _showDeliOptionsModal = false;
    }

    private void ToggleStagedDeliOption(string name)
    {
        if (!_stagedDeliSelections.Remove(name))
        {
            if (!VM.IsCardOrder)
            {
                var optionType = VM.OptionTypes.FirstOrDefault(o => o.OptionType.Id == _activeDeliOptionTypeId);
                if (optionType != null && _stagedDeliSelections.Count >= optionType.OptionType.NumIncluded)
                    return;
            }
            _stagedDeliSelections.Add(name);
        }
        StateHasChanged();
    }

    private void SetStagedDeliOption(string name)
    {
        _stagedDeliSelections.Clear();
        _stagedDeliSelections.Add(name);
        StateHasChanged();
    }

    private void ConfirmDeliOptions()
    {
        var currentSelections = VM.GetSelectedOptionsForType(_activeDeliOptionTypeId);
        var activeOptionType = VM.OptionTypes.FirstOrDefault(o => o.OptionType.Id == _activeDeliOptionTypeId);
        var isMulti = activeOptionType != null && VM.IsMultiSelectOptionType(activeOptionType);

        if (isMulti)
        {
            var toRemove = currentSelections.Except(_stagedDeliSelections).ToList();
            var toAdd = _stagedDeliSelections.Except(currentSelections).ToList();
            foreach (var opt in toRemove)
                VM.ToggleOptionForType(_activeDeliOptionTypeId, opt);
            foreach (var opt in toAdd)
                VM.ToggleOptionForType(_activeDeliOptionTypeId, opt);
        }
        else
        {
            var selected = _stagedDeliSelections.FirstOrDefault();
            if (selected != null)
                VM.SetOptionForType(_activeDeliOptionTypeId, selected);
        }

        _showDeliOptionsModal = false;
        StateHasChanged();
    }

    private void SetStagedBreakfastOption(int optionTypeId, string optionName)
    {
        _stagedBreakfastOptions[optionTypeId] = optionName;
        StateHasChanged();
    }

    private void ConfirmBreakfastOptions()
    {
        if (_stagedEntree != null)
        {
            VM.State.SelectedEntree = _stagedEntree;
            _stagedEntree = null;
        }

        foreach (var kvp in _stagedBreakfastOptions)
        {
            VM.SetOptionForType(kvp.Key, kvp.Value);
        }
        _showOptionsModal = false;
        StateHasChanged();
    }

    private void OpenPizzaToppingsModal()
    {
        _stagedToppings = new HashSet<string>(VM.State.SelectedToppings);
        _showPizzaToppingsModal = true;
    }

    private void ClosePizzaToppingsModal()
    {
        _stagedEntree = null;
        _stagedToppings.Clear();
        _showPizzaToppingsModal = false;
        StateHasChanged();
    }

    private void ToggleStagedTopping(string topping)
    {
        if (!_stagedToppings.Remove(topping))
        {
            if (!VM.IsCardOrder)
            {
                var toppingsOptionType = VM.OptionTypes.FirstOrDefault();
                if (toppingsOptionType != null && _stagedToppings.Count >= toppingsOptionType.OptionType.NumIncluded)
                    return;
            }
            _stagedToppings.Add(topping);
        }
        StateHasChanged();
    }

    private void ConfirmPizzaToppings()
    {
        if (_stagedEntree != null)
        {
            VM.State.SelectedEntree = _stagedEntree;
            _stagedEntree = null;
        }

        var toRemove = VM.State.SelectedToppings.Except(_stagedToppings).ToList();
        var toAdd = _stagedToppings.Except(VM.State.SelectedToppings).ToList();

        foreach (var topping in toRemove)
            VM.ToggleTopping(topping);
        foreach (var topping in toAdd)
            VM.ToggleTopping(topping);

        _showPizzaToppingsModal = false;
        StateHasChanged();
    }

    private void SelectSide(SideDto side)
    {
        VM.SelectSide(side);
        StateHasChanged();
    }

    private void SelectDrink(DrinkDto drink)
    {
        VM.SelectDrink(drink);
        StateHasChanged();
    }

    private void SetOptionForType(int optionTypeId, string optionName)
    {
        VM.SetOptionForType(optionTypeId, optionName);
        StateHasChanged();
    }

    private void ToggleOptionForType(int optionTypeId, string optionName)
    {
        VM.ToggleOptionForType(optionTypeId, optionName);
        StateHasChanged();
    }

    private void ToggleTopping(string topping)
    {
        VM.ToggleTopping(topping);
        StateHasChanged();
    }

    private async Task AddToOrder()
    {
        var success = await VM.AddToOrderAsync();
        if (success)
        {
            NavigationManager.NavigateTo("/place-order");
        }
    }

    private bool IsTabCompleted(string tabId)
    {
        return tabId switch
        {
            "entrees" => VM.State.SelectedEntree != null,
            "sandwich" => IsSandwichComplete(),
            "wrap" => IsWrapComplete(),
            "toppings" => VM.State.SelectedEntree != null &&
                          VM.State.SelectedToppings.Count >= (VM.Configuration?.MinimumToppingsRequired ?? 0),
            "sides" => VM.State.SelectedSide != null,
            "drinks" => VM.State.SelectedDrink != null || !VM.Drinks.Any(),
            _ => false
        };
    }

    private bool IsSandwichComplete()
    {
        if (!VM.OptionTypes.Any())
            return false;

        foreach (var optionType in VM.OptionTypes)
        {
            var requiredCount = optionType.OptionType.NumIncluded;
            var selectedCount = VM.State.MultiSelectOptions.TryGetValue(optionType.OptionType.Id, out var selected)
                ? selected.Count
                : 0;

            if (selectedCount < requiredCount)
                return false;
        }

        return true;
    }

    private bool IsWrapComplete()
    {
        if (!VM.OptionTypes.Any())
            return false;

        foreach (var optionType in VM.OptionTypes)
        {
            var requiredCount = optionType.OptionType.NumIncluded;
            var selectedCount = VM.State.MultiSelectOptions.TryGetValue(optionType.OptionType.Id, out var selected)
                ? selected.Count
                : 0;

            if (selectedCount < requiredCount)
                return false;
        }

        return true;
    }

    private bool IsLastTab()
    {
        var tabs = VM.Configuration?.Tabs ?? new();
        return tabs.Count == 0 || tabs.Last().Id == VM.ActiveTab;
    }

    private void GoToNextTab()
    {
        var tabs = VM.Configuration?.Tabs ?? new();
        var currentIndex = tabs.FindIndex(t => t.Id == VM.ActiveTab);
        if (currentIndex >= 0 && currentIndex < tabs.Count - 1)
        {
            VM.SetActiveTab(tabs[currentIndex + 1].Id);
            StateHasChanged();
        }
    }

    private bool AreAllTabsCompleted()
    {
        var tabs = VM.Configuration?.Tabs ?? new();
        return tabs.All(tab => IsTabCompleted(tab.Id));
    }

    private bool HasAnySelection()
    {
        return VM.State.SelectedEntree != null ||
               VM.State.SelectedSide != null ||
               VM.State.SelectedDrink != null ||
               VM.State.SelectedToppings.Count > 0 ||
               VM.State.MultiSelectOptions.Values.Any(list => list.Count > 0);
    }

    private bool IsFirstTab()
    {
        var tabs = VM.Configuration?.Tabs ?? new();
        return tabs.Count == 0 || tabs.First().Id == VM.ActiveTab;
    }

    private void GoToPreviousTab()
    {
        var tabs = VM.Configuration?.Tabs ?? new();
        var currentIndex = tabs.FindIndex(t => t.Id == VM.ActiveTab);
        if (currentIndex > 0)
        {
            VM.SetActiveTab(tabs[currentIndex - 1].Id);
            StateHasChanged();
        }
    }

    private string GetTabIcon(string tabId)
    {
        return tabId switch
        {
            "entrees" => "bi-egg-fried",
            "sandwich" => "bi-stack",
            "wrap" => "bi-tornado",
            "toppings" => "bi-circle-fill",
            "sides" => "bi-basket2-fill",
            "drinks" => "bi-cup-straw",
            _ => "bi-circle"
        };
    }

    private bool HasSelectionForTab(string tabId)
    {
        return tabId switch
        {
            "entrees" => VM.State.SelectedEntree != null,
            "sandwich" => VM.State.MultiSelectOptions.Values.Any(list => list.Count > 0),
            "wrap" => VM.State.MultiSelectOptions.Values.Any(list => list.Count > 0),
            "toppings" => VM.State.SelectedToppings.Count > 0,
            "sides" => VM.State.SelectedSide != null,
            "drinks" => VM.State.SelectedDrink != null,
            _ => false
        };
    }

    private string GetSelectionTextForTab(string tabId)
    {
        return tabId switch
        {
            "entrees" => VM.State.SelectedEntree?.EntreeName ?? "",
            "sandwich" => GetSandwichSummary(),
            "wrap" => GetWrapSummary(),
            "toppings" => $"{VM.State.SelectedToppings.Count} topping(s)",
            "sides" => VM.State.SelectedSide?.SideName ?? "",
            "drinks" => VM.State.SelectedDrink?.DrinkName ?? "",
            _ => ""
        };
    }

    private string GetSandwichSummary()
    {
        var totalSelections = VM.State.MultiSelectOptions.Values.Sum(list => list.Count);
        if (totalSelections == 0) return "";
        return $"{totalSelections} option(s) selected";
    }

    private string GetWrapSummary()
    {
        var totalSelections = VM.State.MultiSelectOptions.Values.Sum(list => list.Count);
        if (totalSelections == 0) return "";
        return $"{totalSelections} filling(s) selected";
    }

    private string GetStepHint()
    {
        return VM.ActiveTab switch
        {
            "entrees" => "Pick one entree to start your meal.",
            "toppings" => "Select your pizza and choose your toppings.",
            "sandwich" => "Tap the sandwich card to build your custom sandwich.",
            "wrap" => "Build your wrap by selecting your fillings.",
            "sides" => "Pick a side to go with your meal.",
            "drinks" => VM.Drinks.Any()
                ? "Choose a drink to complete your meal."
                : "A fountain drink is included with your meal.",
            _ => ""
        };
    }

    private string GetCardStepHint()
    {
        return VM.ActiveTab switch
        {
            "entrees" => "Browse entrees and add any you'd like.",
            "toppings" => "Pick your pizza, prices shown per item.",
            "sandwich" => "Build your sandwich, extras may have an additional charge.",
            "wrap" => "Build your wrap, extras may have an additional charge.",
            "sides" => "Add a side, prices shown per item.",
            "drinks" => VM.Drinks.Any()
                ? "Add a drink, prices shown per item."
                : "A fountain drink is available at the station.",
            _ => ""
        };
    }
}
