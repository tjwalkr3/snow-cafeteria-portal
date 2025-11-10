using Cafeteria.Customer.Services;
using Cafeteria.Shared.DTOs;

namespace Cafeteria.Customer.Components.Pages.Stations.BreakfastSwipe;

public class BreakfastSwipeVM : IBreakfastSwipeVM
{
    private readonly IApiMenuService _menuService;
    private readonly ICartService _cartService;
    private const string CART_KEY = "order";

    public BreakfastSwipeVM(IApiMenuService menuService, ICartService cartService)
    {
        _menuService = menuService;
        _cartService = cartService;
    }

    public List<EntreeDto> Entrees { get; private set; } = new();
    public List<SideDto> Sides { get; private set; } = new();
    public List<DrinkDto> Drinks { get; private set; } = new();
    public List<FoodOptionDto> CurrentEntreeOptions { get; private set; } = new();

    public string ActiveTab { get; private set; } = "entrees";
    public EntreeDto? SelectedEntree { get; private set; }
    public SideDto? SelectedSide { get; private set; }
    public DrinkDto? SelectedDrink { get; private set; }

    public string? SelectedMeatOption { get; private set; }
    public string? SelectedBreadOption { get; private set; }

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

    public async Task SelectEntree(EntreeDto entree)
    {
        SelectedEntree = entree;
        SelectedMeatOption = null;
        SelectedBreadOption = null;
        CurrentEntreeOptions = await _menuService.GetOptionsByEntree(entree.Id);
    }

    public void SelectSide(SideDto side)
    {
        SelectedSide = side;
    }

    public void SelectDrink(DrinkDto drink)
    {
        SelectedDrink = drink;
    }

    public void SetMeatOption(string meat)
    {
        SelectedMeatOption = meat;
    }

    public void SetBreadOption(string bread)
    {
        SelectedBreadOption = bread;
    }

    public bool RequiresMeatSelection(int entreeId)
    {
        return entreeId == 1 || entreeId == 2 || entreeId == 4 || entreeId == 7;
    }

    public bool RequiresBreadSelection(int entreeId)
    {
        return entreeId == 1;
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
        if (SelectedEntree == null || SelectedSide == null || SelectedDrink == null)
            return false;

        if (RequiresMeatSelection(SelectedEntree.Id) && string.IsNullOrEmpty(SelectedMeatOption))
            return false;

        if (RequiresBreadSelection(SelectedEntree.Id) && string.IsNullOrEmpty(SelectedBreadOption))
            return false;

        return true;
    }

    public async Task<bool> AddToOrderAsync()
    {
        if (!IsValidSelection() || SelectedEntree == null || SelectedSide == null || SelectedDrink == null)
            return false;

        await _cartService.AddEntree(CART_KEY, SelectedEntree);

        if (!string.IsNullOrEmpty(SelectedBreadOption))
        {
            var breadOption = CurrentEntreeOptions.FirstOrDefault(o => o.FoodOptionName == SelectedBreadOption);
            if (breadOption != null)
            {
                var optionType = new FoodOptionTypeDto
                {
                    FoodOptionTypeName = "Bread Choice",
                    EntreeId = SelectedEntree.Id
                };
                await _cartService.AddEntreeOption(CART_KEY, SelectedEntree.Id, breadOption, optionType);
            }
        }

        if (!string.IsNullOrEmpty(SelectedMeatOption))
        {
            var meatOption = CurrentEntreeOptions.FirstOrDefault(o => o.FoodOptionName == SelectedMeatOption);
            if (meatOption != null)
            {
                var optionType = new FoodOptionTypeDto
                {
                    FoodOptionTypeName = "Meat Choice",
                    EntreeId = SelectedEntree.Id
                };
                await _cartService.AddEntreeOption(CART_KEY, SelectedEntree.Id, meatOption, optionType);
            }
        }

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
        SelectedMeatOption = null;
        SelectedBreadOption = null;
        CurrentEntreeOptions.Clear();
        ActiveTab = "entrees";
    }
}
