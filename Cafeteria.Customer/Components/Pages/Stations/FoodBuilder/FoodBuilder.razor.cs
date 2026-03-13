using Cafeteria.Customer.Components.Pages.Stations.Domain;
using Cafeteria.Customer.Services.Cart;
using Cafeteria.Customer.Services.Menu;
using Cafeteria.Shared.DTOs.Menu;
using Microsoft.AspNetCore.Components;

namespace Cafeteria.Customer.Components.Pages.Stations.FoodBuilder;

public partial class FoodBuilder : ComponentBase
{
    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    [Inject]
    private ICartService Cart { get; set; } = default!;

    [Inject]
    private IApiMenuService MenuService { get; set; } = default!;

    [Inject]
    private CartSubmitter CartSubmitter { get; set; } = default!;

    [Inject]
    private FoodOptionStagingStore StagingStore { get; set; } = default!;

    private List<EntreeDto> Entrees { get; set; } = new();
    private List<SideWithOptionsDto> Sides { get; set; } = new();
    private List<DrinkDto> Drinks { get; set; } = new();
    private List<FoodOptionTypeWithOptionsDto> OptionTypes { get; set; } = new();
    private List<TabDefinition> Tabs { get; set; } = new();
    private string PageTitle { get; set; } = string.Empty;
    private SelectionState State { get; } = new();
    private string ActiveTab { get; set; } = "entrees";
    private bool IsCardOrder { get; set; }

    private bool _isLoading = true;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            var order = await Cart.GetOrder("order");
            int stationId = order?.StationId ?? 0;
            int locationId = order?.Location?.Id ?? 0;
            IsCardOrder = order?.IsCardOrder ?? false;
            PageTitle = string.IsNullOrEmpty(order?.StationName) ? "Station" : order.StationName;

            State.Clear();
            Entrees = await MenuService.GetEntreesByStation(stationId);
            Sides = await MenuService.GetSidesWithOptionsByStation(stationId);
            Drinks = await MenuService.GetDrinksByLocation(locationId);
            OptionTypes = new List<FoodOptionTypeWithOptionsDto>();
            Tabs = new List<TabDefinition>
            {
                new("entrees", "Entrees", isDefault: true),
                new("sides", "Sides"),
                new("drinks", "Drinks")
            };
            ActiveTab = "entrees";

            _isLoading = false;
            StateHasChanged();
        }
    }

    public string CreateBackUrl() => "/station-select";

    private void SetActiveTab(string tab)
    {
        ActiveTab = tab;
        StateHasChanged();
    }

    private async Task SelectEntree(EntreeDto entree)
    {
        OptionTypes = await MenuService.GetOptionTypesWithOptionsByEntree(entree.Id);

        if (OptionTypes.Any())
            StagingStore.Open(entree, OptionTypes, State);
        else
        {
            State.SelectedEntree = entree;
            State.ClearOptionsOnly();
        }

        StateHasChanged();
    }

    private void ConfirmOptions()
    {
        var optionTypes = StagingStore.StagedSide?.OptionTypes ?? OptionTypes;
        StagingStore.Confirm(State, optionTypes);
        StateHasChanged();
    }

    private void DiscardOptions()
    {
        StagingStore.Discard();
        StateHasChanged();
    }

    private void SelectSide(SideDto side)
    {
        State.SelectedSide = side;
        StateHasChanged();
    }

    private void SelectSideWithOptions(SideWithOptionsDto side)
    {
        StagingStore.OpenForSide(side, State);
        StateHasChanged();
    }

    private void SelectDrink(DrinkDto drink)
    {
        State.SelectedDrink = drink;
        StateHasChanged();
    }

    private async Task AddToOrder()
    {
        if (!SelectionValidator.IsValid(State, OptionTypes, IsCardOrder))
            return;

        var sideWithOptions = Sides.FirstOrDefault(s => s.Side.Id == State.SelectedSide?.Id);
        await CartSubmitter.SubmitAsync(State, OptionTypes, new List<FoodOptionDto>(), sideWithOptions?.OptionTypes);
        State.Clear();
        ActiveTab = Tabs.FirstOrDefault()?.Id ?? "entrees";
        NavigationManager.NavigateTo("/place-order");
    }

    private bool IsTabCompleted(string tabId)
    {
        return tabId switch
        {
            "entrees" => State.SelectedEntree != null,
            "sides" => State.SelectedSide != null,
            "drinks" => State.SelectedDrink != null || !Drinks.Any(),
            _ => false
        };
    }

    private bool IsLastTab()
    {
        return Tabs.Count == 0 || Tabs.Last().Id == ActiveTab;
    }

    private void GoToNextTab()
    {
        var currentIndex = Tabs.FindIndex(t => t.Id == ActiveTab);
        if (currentIndex >= 0 && currentIndex < Tabs.Count - 1)
        {
            ActiveTab = Tabs[currentIndex + 1].Id;
            StateHasChanged();
        }
    }

    private bool AreAllTabsCompleted()
    {
        return Tabs.All(tab => IsTabCompleted(tab.Id));
    }

    private bool HasAnySelection()
    {
        return State.SelectedEntree != null ||
               State.SelectedSide != null ||
               State.SelectedDrink != null ||
               State.MultiSelectOptions.Values.Any(list => list.Count > 0) ||
               State.SingleSelectOptions.Any();
    }

    private bool IsFirstTab()
    {
        return Tabs.Count == 0 || Tabs.First().Id == ActiveTab;
    }

    private void GoToPreviousTab()
    {
        var currentIndex = Tabs.FindIndex(t => t.Id == ActiveTab);
        if (currentIndex > 0)
        {
            ActiveTab = Tabs[currentIndex - 1].Id;
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
            "entrees" => State.SelectedEntree != null,
            "sides" => State.SelectedSide != null,
            "drinks" => State.SelectedDrink != null,
            _ => false
        };
    }

    private string GetSelectionTextForTab(string tabId)
    {
        return tabId switch
        {
            "entrees" => State.SelectedEntree?.EntreeName ?? "",
            "sides" => State.SelectedSide?.SideName ?? "",
            "drinks" => State.SelectedDrink?.DrinkName ?? "",
            _ => ""
        };
    }

    private string GetStepHint()
    {
        return ActiveTab switch
        {
            "entrees" => "Pick one entree to start your meal.",
            "sides" => "Pick a side to go with your meal.",
            "drinks" => Drinks.Any()
                ? "Choose a drink to complete your meal."
                : "A fountain drink is included with your meal.",
            _ => ""
        };
    }

    private string GetCardStepHint()
    {
        return ActiveTab switch
        {
            "entrees" => "Browse entrees and add any you'd like.",
            "sides" => "Add a side, prices shown per item.",
            "drinks" => Drinks.Any()
                ? "Add a drink, prices shown per item."
                : "A fountain drink is available at the station.",
            _ => ""
        };
    }
}
