using Cafeteria.Customer.Services;
using Cafeteria.Shared.DTOs;

namespace Cafeteria.Customer.Components.Pages.Stations.PizzaSwipe;

public class PizzaSwipeVM : IPizzaSwipeVM
{
    private readonly IApiMenuService _menuService;
    private readonly ICartService _cartService;
    private const string CART_KEY = "order";

    public PizzaSwipeVM(IApiMenuService menuService, ICartService cartService)
    {
        _menuService = menuService;
        _cartService = cartService;
    }

    public List<EntreeDto> Entrees { get; private set; } = new();
    public List<DrinkDto> Drinks { get; private set; } = new();
    public List<FoodOptionDto> AllEntreeOptions { get; private set; } = new();

    public string ActiveTab { get; private set; } = "toppings";
    public EntreeDto? SelectedEntree { get; private set; }
    public DrinkDto? SelectedDrink { get; private set; }
    public List<string> SelectedToppings { get; private set; } = new();

    public List<string> AvailableToppings { get; private set; } = new();

    public int StationId { get; set; }
    public int LocationId { get; set; }

    public async Task LoadDataAsync(int stationId, int locationId)
    {
        StationId = stationId;
        LocationId = locationId;

        Entrees = await _menuService.GetEntreesByStation(stationId);
        Drinks = await _menuService.GetDrinksByLocation(locationId);

        var pizzaEntree = Entrees.FirstOrDefault();
        if (pizzaEntree != null)
        {
            AllEntreeOptions = await _menuService.GetOptionsByEntree(pizzaEntree.Id);
            AvailableToppings = AllEntreeOptions.Select(o => o.FoodOptionName).ToList();

            if (!AvailableToppings.Any())
            {
                AvailableToppings = new List<string>
                {
                    "Extra Cheese", "Pepperoni", "Sausage", "Bacon", "Chicken", "Ham",
                    "Olives", "Mushrooms", "Onions", "Pineapple", "Bell Peppers", "Banana Peppers"
                };
            }
        }

        if (Entrees.Any())
        {
            SelectedEntree = Entrees.First();
        }
    }

    public void SetActiveTab(string tab)
    {
        ActiveTab = tab;
    }

    public void SelectEntree(EntreeDto entree)
    {
        SelectedEntree = entree;
    }

    public void SelectDrink(DrinkDto drink)
    {
        SelectedDrink = drink;
    }

    public void ToggleTopping(string topping)
    {
        if (SelectedToppings.Contains(topping))
        {
            SelectedToppings.Remove(topping);
        }
        else
        {
            SelectedToppings.Add(topping);
        }
    }

    public int GetSelectionCount()
    {
        int count = 0;
        if (SelectedToppings.Count >= 2) count++;
        if (SelectedDrink != null) count++;
        return count;
    }

    public bool IsValidSelection()
    {
        return SelectedToppings.Count >= 2 && SelectedDrink != null;
    }

    public async Task<bool> AddToOrderAsync()
    {
        if (!IsValidSelection() || SelectedDrink == null)
            return false;

        if (SelectedEntree == null && Entrees.Any())
        {
            SelectedEntree = Entrees.First();
        }

        if (SelectedEntree == null)
            return false;

        await _cartService.AddEntree(CART_KEY, SelectedEntree);

        foreach (var topping in SelectedToppings)
        {
            var toppingOption = AllEntreeOptions.FirstOrDefault(o => o.FoodOptionName == topping);
            if (toppingOption != null)
            {
                var optionType = new FoodOptionTypeDto
                {
                    FoodOptionTypeName = "Pizza Toppings",
                    EntreeId = SelectedEntree.Id
                };
                await _cartService.AddEntreeOption(CART_KEY, SelectedEntree.Id, toppingOption, optionType);
            }
        }

        await _cartService.AddDrink(CART_KEY, SelectedDrink);

        ClearSelections();
        return true;
    }

    private void ClearSelections()
    {
        SelectedDrink = null;
        SelectedToppings.Clear();
        ActiveTab = "toppings";

        if (Entrees.Any())
        {
            SelectedEntree = Entrees.First();
        }
    }
}
