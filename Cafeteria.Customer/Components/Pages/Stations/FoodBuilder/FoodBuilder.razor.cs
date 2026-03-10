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
    private NavigationManager NavigationManager { get; set; } = default!;

    [Inject]
    private ICartService Cart { get; set; } = default!;

    [Inject]
    private FoodOptionStagingStore StagingStore { get; set; } = default!;

    private bool _isLoading = true;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            var order = await Cart.GetOrder("order");
            int stationId = order?.StationId ?? 0;
            int locationId = order?.Location?.Id ?? 0;
            bool isCardOrder = order?.IsCardOrder ?? false;
            string stationName = order?.StationName ?? "";

            await VM.InitializeAsync(stationId, locationId, isCardOrder, stationName);
            _isLoading = false;
            StateHasChanged();
        }
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
        var optionTypes = StagingStore.StagedSide?.OptionTypes ?? VM.OptionTypes;
        StagingStore.Confirm(VM.State, optionTypes);
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

    private void SelectSideWithOptions(SideWithOptionsDto side)
    {
        StagingStore.OpenForSide(side, VM.State);
        StateHasChanged();
    }

    private void SelectDrink(DrinkDto drink)
    {
        VM.SelectDrink(drink);
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
            "sides" => VM.State.SelectedSide != null,
            "drinks" => VM.State.SelectedDrink != null || !VM.Drinks.Any(),
            _ => false
        };
    }

    private bool IsLastTab()
    {
        return VM.Tabs.Count == 0 || VM.Tabs.Last().Id == VM.ActiveTab;
    }

    private void GoToNextTab()
    {
        var currentIndex = VM.Tabs.FindIndex(t => t.Id == VM.ActiveTab);
        if (currentIndex >= 0 && currentIndex < VM.Tabs.Count - 1)
        {
            VM.SetActiveTab(VM.Tabs[currentIndex + 1].Id);
            StateHasChanged();
        }
    }

    private bool AreAllTabsCompleted()
    {
        return VM.Tabs.All(tab => IsTabCompleted(tab.Id));
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
        return VM.Tabs.Count == 0 || VM.Tabs.First().Id == VM.ActiveTab;
    }

    private void GoToPreviousTab()
    {
        var currentIndex = VM.Tabs.FindIndex(t => t.Id == VM.ActiveTab);
        if (currentIndex > 0)
        {
            VM.SetActiveTab(VM.Tabs[currentIndex - 1].Id);
            StateHasChanged();
        }
    }

    private string GetTabIcon(string tabId)
    {
        return tabId switch
        {
            "entrees" => "bi-egg-fried",
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
            "sides" => VM.State.SelectedSide?.SideName ?? "",
            "drinks" => VM.State.SelectedDrink?.DrinkName ?? "",
            _ => ""
        };
    }

    private string GetStepHint()
    {
        return VM.ActiveTab switch
        {
            "entrees" => "Pick one entree to start your meal.",
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
            "sides" => "Add a side, prices shown per item.",
            "drinks" => VM.Drinks.Any()
                ? "Add a drink, prices shown per item."
                : "A fountain drink is available at the station.",
            _ => ""
        };
    }
}
