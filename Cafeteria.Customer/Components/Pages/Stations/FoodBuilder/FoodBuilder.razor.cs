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

    private int _entreeQuantity = 0;
    private int _sideQuantity = 0;
    private int _drinkQuantity = 0;

    private Dictionary<int, int> _cardEntreeQtys = new();
    private Dictionary<int, int> _cardSideQtys = new();
    private Dictionary<int, int> _cardDrinkQtys = new();

    private Dictionary<int, Dictionary<int, string>> _cardEntreeSingleOpts = new();
    private Dictionary<int, Dictionary<int, List<string>>> _cardEntreeMultiOpts = new();
    private Dictionary<int, List<FoodOptionTypeWithOptionsDto>> _cardEntreeOptionTypes = new();

    private Dictionary<int, Dictionary<int, HashSet<string>>> _cardSideOpts = new();

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

            if (IsCardOrder)
            {
                Entrees = await MenuService.GetCardEntreesByStation(stationId);
                Sides = await MenuService.GetCardSidesByStation(stationId);
                Drinks = await MenuService.GetCardDrinksByLocation(locationId);
            }
            else
            {
                Entrees = await MenuService.GetSwipeEntreesByStation(stationId);
                Sides = await MenuService.GetSwipeSidesByStation(stationId);
                Drinks = await MenuService.GetSwipeDrinksByLocation(locationId);
            }

            OptionTypes = new List<FoodOptionTypeWithOptionsDto>();
            Tabs = new List<TabDefinition> { new("entrees", "Entrees", isDefault: true) };
            if (Sides.Any())
                Tabs.Add(new("sides", "Sides"));
            Tabs.Add(new("drinks", "Drinks"));
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
        if (IsCardOrder)
        {
            if (_cardEntreeQtys.ContainsKey(entree.Id))
            {
                _cardEntreeQtys[entree.Id]++;
                StateHasChanged();
                return;
            }
            OptionTypes = await MenuService.GetOptionTypesWithOptionsByEntree(entree.Id);
            _cardEntreeOptionTypes[entree.Id] = OptionTypes;
            if (OptionTypes.Any())
                StagingStore.Open(entree, OptionTypes, new SelectionState());
            else
                _cardEntreeQtys[entree.Id] = 1;
            StateHasChanged();
            return;
        }

        OptionTypes = await MenuService.GetOptionTypesWithOptionsByEntree(entree.Id);
        if (OptionTypes.Any())
        {
            StagingStore.Open(entree, OptionTypes, State);
        }
        else
        {
            State.SelectedEntree = entree;
            State.ClearOptionsOnly();
            _entreeQuantity = 1;
        }
        StateHasChanged();
    }

    private void ConfirmOptions()
    {
        if (IsCardOrder)
        {
            if (StagingStore.StagedSide != null)
            {
                var sideId = StagingStore.StagedSide.Side.Id;
                _cardSideOpts[sideId] = new Dictionary<int, HashSet<string>>();
                foreach (var ot in StagingStore.StagedSide.OptionTypes)
                {
                    var id = ot.OptionType.Id;
                    _cardSideOpts[sideId][id] = new HashSet<string>(
                        StagingStore.StagedSelections.GetValueOrDefault(id) ?? new HashSet<string>());
                }
                if (!_cardSideQtys.ContainsKey(sideId))
                    _cardSideQtys[sideId] = 1;
            }
            else if (StagingStore.StagedEntree != null)
            {
                var entreeId = StagingStore.StagedEntree.Id;
                _cardEntreeSingleOpts[entreeId] = new Dictionary<int, string>();
                _cardEntreeMultiOpts[entreeId] = new Dictionary<int, List<string>>();
                foreach (var ot in OptionTypes)
                {
                    var id = ot.OptionType.Id;
                    var staged = StagingStore.StagedSelections.GetValueOrDefault(id) ?? new HashSet<string>();
                    if (ot.OptionType.MaxAmount > 1)
                        _cardEntreeMultiOpts[entreeId][id] = staged.ToList();
                    else
                    {
                        var first = staged.FirstOrDefault();
                        if (first != null) _cardEntreeSingleOpts[entreeId][id] = first;
                    }
                }
                if (!_cardEntreeQtys.ContainsKey(entreeId))
                    _cardEntreeQtys[entreeId] = 1;
            }
            StagingStore.Discard();
            StateHasChanged();
            return;
        }

        bool isSide = StagingStore.StagedSide != null;
        if (!isSide)
            State.ClearOptionsOnly();
        var optionTypes = StagingStore.StagedSide?.OptionTypes ?? OptionTypes;
        StagingStore.Confirm(State, optionTypes);
        if (isSide)
            _sideQuantity = 1;
        else
            _entreeQuantity = 1;
        StateHasChanged();
    }

    private void DiscardOptions()
    {
        StagingStore.Discard();
        StateHasChanged();
    }

    private void SelectSide(SideDto side)
    {
        if (IsCardOrder)
        {
            _cardSideQtys[side.Id] = _cardSideQtys.GetValueOrDefault(side.Id, 0) + 1;
            StateHasChanged();
            return;
        }
        State.SelectedSide = side;
        _sideQuantity = 1;
        StateHasChanged();
    }

    private void SelectSideWithOptions(SideWithOptionsDto side)
    {
        if (IsCardOrder)
        {
            if (_cardSideQtys.ContainsKey(side.Side.Id))
            {
                _cardSideQtys[side.Side.Id]++;
                StateHasChanged();
                return;
            }
            var tempState = new SelectionState();
            if (_cardSideOpts.TryGetValue(side.Side.Id, out var existing))
                foreach (var kv in existing) tempState.SideOptions[kv.Key] = new HashSet<string>(kv.Value);
            StagingStore.OpenForSide(side, tempState);
            StateHasChanged();
            return;
        }
        StagingStore.OpenForSide(side, State);
        StateHasChanged();
    }

    private void SelectDrink(DrinkDto drink)
    {
        if (IsCardOrder)
        {
            _cardDrinkQtys[drink.Id] = _cardDrinkQtys.GetValueOrDefault(drink.Id, 0) + 1;
            StateHasChanged();
            return;
        }
        State.SelectedDrink = drink;
        _drinkQuantity = 1;
        StateHasChanged();
    }

    private void ChangeCardEntreeQty(int entreeId, int newQty)
    {
        if (newQty <= 0)
        {
            _cardEntreeQtys.Remove(entreeId);
            _cardEntreeSingleOpts.Remove(entreeId);
            _cardEntreeMultiOpts.Remove(entreeId);
            _cardEntreeOptionTypes.Remove(entreeId);
        }
        else
            _cardEntreeQtys[entreeId] = newQty;
        StateHasChanged();
    }

    private void ChangeCardSideQty(int sideId, int newQty)
    {
        if (newQty <= 0)
        {
            _cardSideQtys.Remove(sideId);
            _cardSideOpts.Remove(sideId);
        }
        else
            _cardSideQtys[sideId] = newQty;
        StateHasChanged();
    }

    private void ChangeCardDrinkQty(int drinkId, int newQty)
    {
        if (newQty <= 0)
            _cardDrinkQtys.Remove(drinkId);
        else
            _cardDrinkQtys[drinkId] = newQty;
        StateHasChanged();
    }

    private void ChangeEntreeQuantity(int newQty)
    {
        _entreeQuantity = Math.Max(0, newQty);
        if (_entreeQuantity == 0)
        {
            State.SelectedEntree = null;
            State.ClearOptionsOnly();
        }
        StateHasChanged();
    }

    private void ChangeSideQuantity(int newQty)
    {
        _sideQuantity = Math.Max(0, newQty);
        if (_sideQuantity == 0)
            State.SelectedSide = null;
        StateHasChanged();
    }

    private void ChangeDrinkQuantity(int newQty)
    {
        _drinkQuantity = Math.Max(0, newQty);
        if (_drinkQuantity == 0)
            State.SelectedDrink = null;
        StateHasChanged();
    }

    private async Task AddToOrder()
    {
        if (!IsCardOrder && !SelectionValidator.IsValid(State, OptionTypes, false))
            return;
        if (IsCardOrder && !HasAnySelection())
            return;

        if (!IsCardOrder)
        {
            var sideWithOptions = Sides.FirstOrDefault(s => s.Side.Id == State.SelectedSide?.Id);
            await CartSubmitter.SubmitAsync(State, OptionTypes, new List<FoodOptionDto>(), sideWithOptions?.OptionTypes);
        }
        else
        {
            foreach (var (entreeId, qty) in _cardEntreeQtys)
            {
                var entree = Entrees.FirstOrDefault(e => e.Id == entreeId);
                if (entree == null) continue;
                var entreeState = new SelectionState { SelectedEntree = entree };
                if (_cardEntreeSingleOpts.TryGetValue(entreeId, out var single))
                    foreach (var kv in single) entreeState.SingleSelectOptions[kv.Key] = kv.Value;
                if (_cardEntreeMultiOpts.TryGetValue(entreeId, out var multi))
                    foreach (var kv in multi) entreeState.MultiSelectOptions[kv.Key] = new List<string>(kv.Value);
                var optTypes = _cardEntreeOptionTypes.GetValueOrDefault(entreeId) ?? new List<FoodOptionTypeWithOptionsDto>();
                for (int i = 0; i < qty; i++)
                    await CartSubmitter.SubmitAsync(entreeState, optTypes, new List<FoodOptionDto>(), null);
            }

            foreach (var (sideId, qty) in _cardSideQtys)
            {
                var sideWithOpts = Sides.FirstOrDefault(s => s.Side.Id == sideId);
                if (sideWithOpts == null) continue;
                var sideState = new SelectionState { SelectedSide = sideWithOpts.Side };
                if (_cardSideOpts.TryGetValue(sideId, out var sideOpts))
                    foreach (var kv in sideOpts) sideState.SideOptions[kv.Key] = new HashSet<string>(kv.Value);
                for (int i = 0; i < qty; i++)
                    await CartSubmitter.SubmitAsync(sideState, new List<FoodOptionTypeWithOptionsDto>(), new List<FoodOptionDto>(), sideWithOpts.OptionTypes);
            }

            foreach (var (drinkId, qty) in _cardDrinkQtys)
            {
                var drink = Drinks.FirstOrDefault(d => d.Id == drinkId);
                if (drink == null) continue;
                var drinkState = new SelectionState { SelectedDrink = drink };
                for (int i = 0; i < qty; i++)
                    await CartSubmitter.SubmitAsync(drinkState, new List<FoodOptionTypeWithOptionsDto>(), new List<FoodOptionDto>(), null);
            }

            _cardEntreeQtys.Clear();
            _cardSideQtys.Clear();
            _cardDrinkQtys.Clear();
            _cardEntreeSingleOpts.Clear();
            _cardEntreeMultiOpts.Clear();
            _cardEntreeOptionTypes.Clear();
            _cardSideOpts.Clear();
        }

        State.Clear();
        _entreeQuantity = 0;
        _sideQuantity = 0;
        _drinkQuantity = 0;
        ActiveTab = Tabs.FirstOrDefault()?.Id ?? "entrees";
        NavigationManager.NavigateTo("/place-order");
    }

    private SelectionState EntreeOnlyState()
    {
        var s = new SelectionState { SelectedEntree = State.SelectedEntree };
        foreach (var kv in State.SingleSelectOptions) s.SingleSelectOptions[kv.Key] = kv.Value;
        foreach (var kv in State.MultiSelectOptions) s.MultiSelectOptions[kv.Key] = new List<string>(kv.Value);
        return s;
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
        if (IsCardOrder)
            return _cardEntreeQtys.Any() || _cardSideQtys.Any() || _cardDrinkQtys.Any();
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
        if (IsCardOrder)
            return tabId switch
            {
                "entrees" => _cardEntreeQtys.Any(),
                "sides" => _cardSideQtys.Any(),
                "drinks" => _cardDrinkQtys.Any(),
                _ => false
            };
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
        if (IsCardOrder)
        {
            return tabId switch
            {
                "entrees" => _cardEntreeQtys.Values.Sum() is int ec and > 0 ? $"{ec} added" : "",
                "sides" => _cardSideQtys.Values.Sum() is int sc and > 0 ? $"{sc} added" : "",
                "drinks" => _cardDrinkQtys.Values.Sum() is int dc and > 0 ? $"{dc} added" : "",
                _ => ""
            };
        }
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
