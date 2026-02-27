using Cafeteria.Customer.Components.Pages.Stations.Configuration;
using Cafeteria.Customer.Components.Pages.Stations.Domain;
using Cafeteria.Customer.Services.Cart;
using Cafeteria.Shared.DTOs.Menu;
using Microsoft.AspNetCore.Components;

namespace Cafeteria.Customer.Components.Pages.Stations.FoodBuilder;

public partial class FoodBuilder : ComponentBase
{
    [Inject]
    private IFoodBuilderVM VM { get; set; } = default!;

    [Inject]
    private IStationConfigurationProvider ConfigProvider { get; set; } = default!;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    [Inject]
    private ICartService Cart { get; set; } = default!;

    [Inject]
    private FoodOptionStagingStore StagingStore { get; set; } = default!;

    [Parameter]
    public string? StationType { get; set; }

    private bool _isLoading = true;

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

    private StationType DetermineStationType()
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

    public string CreateBackUrl() => "/station-select";

    private void SetActiveTab(string tab)
    {
        VM.SetActiveTab(tab);
        StateHasChanged();
    }

    private async Task SelectEntree(EntreeDto entree)
    {
        await VM.SelectEntreeAsync(entree);

        if (VM.OptionTypes.Any())
            StagingStore.Open(entree, VM.OptionTypes, VM.State);

        StateHasChanged();
    }

    private void OpenOptionsModal()
    {
        if (VM.State.SelectedEntree != null && VM.OptionTypes.Any())
            StagingStore.Open(VM.State.SelectedEntree, VM.OptionTypes, VM.State);
    }

    private void ConfirmOptions()
    {
        StagingStore.Confirm(VM.State, VM.OptionTypes);
        StateHasChanged();
    }

    private void DiscardOptions()
    {
        StagingStore.Discard();
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
            "sandwich" => IsBuilderEntreeComplete(),
            "wrap" => IsBuilderEntreeComplete(),
            "toppings" => VM.State.SelectedEntree != null &&
                          VM.State.MultiSelectOptions.Values.Sum(list => list.Count) >= (VM.Configuration?.MinimumToppingsRequired ?? 0),
            "sides" => VM.State.SelectedSide != null,
            "drinks" => VM.State.SelectedDrink != null || !VM.Drinks.Any(),
            _ => false
        };
    }

    private bool IsBuilderEntreeComplete()
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
               VM.State.MultiSelectOptions.Values.Any(list => list.Count > 0) ||
               VM.State.SingleSelectOptions.Any();
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
            "toppings" => VM.State.MultiSelectOptions.Values.Any(list => list.Count > 0),
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
            "sandwich" => GetBuilderSummary("option(s)"),
            "wrap" => GetBuilderSummary("filling(s)"),
            "toppings" => $"{VM.State.MultiSelectOptions.Values.Sum(list => list.Count)} topping(s)",
            "sides" => VM.State.SelectedSide?.SideName ?? "",
            "drinks" => VM.State.SelectedDrink?.DrinkName ?? "",
            _ => ""
        };
    }

    private string GetBuilderSummary(string unit)
    {
        var totalSelections = VM.State.MultiSelectOptions.Values.Sum(list => list.Count);
        if (totalSelections == 0) return "";
        return $"{totalSelections} {unit} selected";
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
