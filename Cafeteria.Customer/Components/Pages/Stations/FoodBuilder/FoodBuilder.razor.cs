using Cafeteria.Customer.Components.Pages.Stations.Domain;
using Cafeteria.Customer.Services.Cart;
using Cafeteria.Customer.Services.Menu;
using Cafeteria.Shared.DTOs.Menu;
using Microsoft.AspNetCore.Components;

namespace Cafeteria.Customer.Components.Pages.Stations.FoodBuilder;

public partial class FoodBuilder : ComponentBase, IDisposable
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

    private CardOrderDraftManager _cardDraft = new();
    private CardOrderAutoSyncCoordinator _cardAutoSync = default!;

    protected override void OnInitialized()
    {
        _cardAutoSync = new CardOrderAutoSyncCoordinator(SyncCardOrderAsync);
    }

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
            if (_cardDraft.ContainsEntree(entree.Id))
            {
                _cardDraft.IncrementEntree(entree.Id);
                ScheduleCardAutoSync();
                StateHasChanged();
                return;
            }
            OptionTypes = await MenuService.GetOptionTypesWithOptionsByEntree(entree.Id);
            if (OptionTypes.Any())
                StagingStore.Open(entree, OptionTypes, new SelectionState());
            else
            {
                _cardDraft.IncrementEntree(entree.Id);
                ScheduleCardAutoSync();
            }
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
                var sideSelections = new Dictionary<int, HashSet<string>>();
                foreach (var ot in StagingStore.StagedSide.OptionTypes)
                {
                    var id = ot.OptionType.Id;
                    sideSelections[id] = new HashSet<string>(
                        StagingStore.StagedSelections.GetValueOrDefault(id) ?? new HashSet<string>());
                }
                _cardDraft.SetSideSelection(sideId, StagingStore.StagedSide.OptionTypes, sideSelections);
            }
            else if (StagingStore.StagedEntree != null)
            {
                var entreeId = StagingStore.StagedEntree.Id;
                var entreeSelections = new Dictionary<int, HashSet<string>>();
                foreach (var ot in OptionTypes)
                {
                    var id = ot.OptionType.Id;
                    var staged = StagingStore.StagedSelections.GetValueOrDefault(id) ?? new HashSet<string>();
                    entreeSelections[id] = new HashSet<string>(staged);
                }
                _cardDraft.SetEntreeSelection(entreeId, OptionTypes, entreeSelections);
            }
            ScheduleCardAutoSync();
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
            _cardDraft.IncrementSide(side.Id);
            ScheduleCardAutoSync();
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
            if (_cardDraft.ContainsSide(side.Side.Id))
            {
                _cardDraft.IncrementSide(side.Side.Id);
                ScheduleCardAutoSync();
                StateHasChanged();
                return;
            }
            var tempState = _cardDraft.CreateSideTempSelectionState(side.Side.Id);
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
            _cardDraft.IncrementDrink(drink.Id);
            ScheduleCardAutoSync();
            StateHasChanged();
            return;
        }
        State.SelectedDrink = drink;
        _drinkQuantity = 1;
        StateHasChanged();
    }

    private void ChangeCardEntreeQty(int entreeId, int newQty)
    {
        _cardDraft.SetEntreeQuantity(entreeId, newQty);
        ScheduleCardAutoSync();
        StateHasChanged();
    }

    private void ChangeCardSideQty(int sideId, int newQty)
    {
        _cardDraft.SetSideQuantity(sideId, newQty);
        ScheduleCardAutoSync();
        StateHasChanged();
    }

    private void ChangeCardDrinkQty(int drinkId, int newQty)
    {
        _cardDraft.SetDrinkQuantity(drinkId, newQty);
        ScheduleCardAutoSync();
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

        if (IsCardOrder)
        {
            await GoToCartForCardOrderAsync();
            return;
        }

        var sideWithOptions = Sides.FirstOrDefault(s => s.Side.Id == State.SelectedSide?.Id);
        await CartSubmitter.SubmitAsync(State, OptionTypes, sideWithOptions?.OptionTypes);

        State.Clear();
        _entreeQuantity = 0;
        _sideQuantity = 0;
        _drinkQuantity = 0;
        ActiveTab = Tabs.FirstOrDefault()?.Id ?? "entrees";
        NavigationManager.NavigateTo("/place-order");
    }

    private async Task GoToCartForCardOrderAsync()
    {
        await FlushCardAutoSyncAsync();
        NavigationManager.NavigateTo("/place-order");
    }

    private void ScheduleCardAutoSync()
    {
        if (!IsCardOrder)
            return;

        _cardAutoSync.Schedule();
    }

    private async Task FlushCardAutoSyncAsync()
    {
        await _cardAutoSync.FlushAsync();
    }

    private async Task SyncCardOrderAsync()
    {
        if (!IsCardOrder)
            return;

        var draft = CreateCardDraftSnapshot();
        var mapped = StationDraftToOrderMapper.MapCardSelections(draft);
        await Cart.UpdateCardOrderItems("order", mapped.Entrees, mapped.Sides, mapped.Drinks);
    }

    private CardStationDraft CreateCardDraftSnapshot()
    {
        return _cardDraft.CreateSnapshot(Entrees, Sides, Drinks);
    }

    public void Dispose()
    {
        _cardAutoSync.Dispose();
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
            return _cardDraft.HasAnySelection();
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
                "entrees" => _cardDraft.HasEntreeSelection(),
                "sides" => _cardDraft.HasSideSelection(),
                "drinks" => _cardDraft.HasDrinkSelection(),
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
                "entrees" => _cardDraft.TotalEntreeCount() is int ec and > 0 ? $"{ec} added" : "",
                "sides" => _cardDraft.TotalSideCount() is int sc and > 0 ? $"{sc} added" : "",
                "drinks" => _cardDraft.TotalDrinkCount() is int dc and > 0 ? $"{dc} added" : "",
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
