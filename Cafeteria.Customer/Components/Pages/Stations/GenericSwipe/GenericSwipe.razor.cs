using Cafeteria.Customer.Components.Pages.Stations.Configuration;
using Cafeteria.Shared.DTOs.Menu;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;

namespace Cafeteria.Customer.Components.Pages.Stations.GenericSwipe;

public partial class GenericSwipe : ComponentBase
{
    [Inject]
    private IGenericSwipeVM VM { get; set; } = default!;

    [Inject]
    private IStationConfigurationProvider ConfigProvider { get; set; } = default!;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    [Parameter]
    public string? StationType { get; set; }

    [SupplyParameterFromQuery(Name = "location")]
    public int Location { get; set; }

    [SupplyParameterFromQuery(Name = "payment")]
    public string? Payment { get; set; }

    [SupplyParameterFromQuery(Name = "station")]
    public int Station { get; set; }

    private bool _isLoading = true;
    private bool _showOptionsModal;
    private bool _showDeliOptionsModal;
    private int _activeDeliOptionTypeId;
    private bool _showPizzaToppingsModal;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            var stationType = DetermineStationType();
            bool isCardOrder = Payment == "card";

            await VM.InitializeAsync(stationType, Station, Location, isCardOrder);
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
            "cheese" => "bi-square-fill",
            "toppings" => "bi-leaf",
            "dressing" => "bi-droplet-fill",
            _ => "bi-circle"
        };
    }

    public string CreateBackUrl()
    {
        Dictionary<string, string?> queryParameters = new() { };

        if (!string.IsNullOrEmpty(Payment))
            queryParameters.Add("payment", Payment);
        queryParameters.Add("location", Location.ToString());

        return QueryHelpers.AddQueryString("/station-select", queryParameters);
    }

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
            _showOptionsModal = true;
        }

        if (VM.CurrentStationType == Configuration.StationType.Pizza)
        {
            _showPizzaToppingsModal = true;
        }

        StateHasChanged();
    }

    private void CloseOptionsModal()
    {
        _showOptionsModal = false;
    }

    private void OpenDeliOptionsModal(int optionTypeId)
    {
        _activeDeliOptionTypeId = optionTypeId;
        _showDeliOptionsModal = true;
    }

    private void CloseDeliOptionsModal()
    {
        _showDeliOptionsModal = false;
    }

    private void OpenPizzaToppingsModal()
    {
        _showPizzaToppingsModal = true;
    }

    private void ClosePizzaToppingsModal()
    {
        _showPizzaToppingsModal = false;
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
            Dictionary<string, string?> queryParameters = new() { };

            if (!string.IsNullOrEmpty(Payment))
                queryParameters.Add("payment", Payment);

            if (Location > 0)
                queryParameters.Add("location", Location.ToString());

            string url = QueryHelpers.AddQueryString("/place-order", queryParameters);
            NavigationManager.NavigateTo(url);
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
}
