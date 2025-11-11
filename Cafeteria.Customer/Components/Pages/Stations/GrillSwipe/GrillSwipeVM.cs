using Cafeteria.Customer.Services;
using Cafeteria.Shared.DTOs;

namespace Cafeteria.Customer.Components.Pages.Stations.GrillSwipe;

public class GrillSwipeVM : IGrillSwipeVM
{
    private readonly IApiMenuService _menuService;
    private readonly ICartService _cartService;
    private const string CART_KEY = "order";

    public GrillSwipeVM(IApiMenuService menuService, ICartService cartService)
    {
        _menuService = menuService;
        _cartService = cartService;
    }

    public List<EntreeDto> Entrees { get; private set; } = new();
    public List<SideDto> Sides { get; private set; } = new();
    public List<DrinkDto> Drinks { get; private set; } = new();

    public string ActiveTab { get; private set; } = "entrees";
    public EntreeDto? SelectedEntree { get; private set; }
    public SideDto? SelectedSide { get; private set; }
    public DrinkDto? SelectedDrink { get; private set; }

    public int StationId { get; set; }
    public int LocationId { get; set; }

    public async Task LoadDataAsync(int stationId, int locationId)
    {
        StationId = stationId;
        LocationId = locationId;

        Entrees = await _menuService.GetEntreesByStation(stationId);
        Sides = await _menuService.GetSidesByStation(stationId);
        Drinks = await _menuService.GetDrinksByLocation(locationId);
    }

    public void SetActiveTab(string tab)
    {
        ActiveTab = tab;
    }

    public void SelectEntree(EntreeDto entree)
    {
        SelectedEntree = entree;
    }

    public void SelectSide(SideDto side)
    {
        SelectedSide = side;
    }

    public void SelectDrink(DrinkDto drink)
    {
        SelectedDrink = drink;
    }

    public int GetSelectionCount()
    {
        int count = 0;
        if (SelectedEntree != null) count++;
        if (SelectedSide != null) count++;
        if (SelectedDrink != null) count++;
        return count;
    }

    public bool IsValidSelection()
    {
        return SelectedEntree != null && SelectedSide != null && SelectedDrink != null;
    }

    public async Task<bool> AddToOrderAsync()
    {
        if (!IsValidSelection() || SelectedEntree == null || SelectedSide == null || SelectedDrink == null)
            return false;

        await _cartService.AddEntree(CART_KEY, SelectedEntree);
        await _cartService.AddSide(CART_KEY, SelectedSide);
        await _cartService.AddDrink(CART_KEY, SelectedDrink);

        ClearSelections();
        return true;
    }

    private void ClearSelections()
    {
        SelectedEntree = null;
        SelectedSide = null;
        SelectedDrink = null;
        ActiveTab = "entrees";
    }
}
