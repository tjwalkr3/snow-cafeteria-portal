using Cafeteria.Customer.Services;
using Cafeteria.Shared.DTOs;

namespace Cafeteria.Customer.Components.Pages.Stations.DeliSwipe;

public class DeliSwipeVM : IDeliSwipeVM
{
    private readonly IApiMenuService _menuService;
    private readonly ICartService _cartService;
    private const string CART_KEY = "order";

    public DeliSwipeVM(IApiMenuService menuService, ICartService cartService)
    {
        _menuService = menuService;
        _cartService = cartService;
    }

    public List<EntreeDto> Entrees { get; private set; } = new();
    public List<SideDto> Sides { get; private set; } = new();
    public List<DrinkDto> Drinks { get; private set; } = new();
    public List<FoodOptionDto> AllEntreeOptions { get; private set; } = new();

    public string ActiveTab { get; private set; } = "sandwich";
    public SideDto? SelectedSide { get; private set; }
    public DrinkDto? SelectedDrink { get; private set; }

    public string? SelectedBread { get; private set; }
    public string? SelectedMeat { get; private set; }
    public string? SelectedCheese { get; private set; }
    public List<string> SelectedToppings { get; private set; } = new();
    public string? SelectedDressing { get; private set; }

    public List<string> BreadOptions { get; private set; } = new();
    public List<string> MeatOptions { get; private set; } = new();
    public List<string> CheeseOptions { get; private set; } = new();
    public List<string> ToppingOptions { get; private set; } = new();
    public List<string> DressingOptions { get; private set; } = new();

    public int StationId { get; set; }
    public int LocationId { get; set; }

    public async Task LoadDataAsync(int stationId, int locationId)
    {
        StationId = stationId;
        LocationId = locationId;

        Entrees = await _menuService.GetEntreesByStation(stationId);
        Sides = await _menuService.GetSidesByStation(stationId);
        Drinks = await _menuService.GetDrinksByLocation(locationId);

        var customSandwichEntree = Entrees.FirstOrDefault(e => e.EntreeName.Contains("Sandwich") || e.EntreeName.Contains("Deli"));
        if (customSandwichEntree != null)
        {
            AllEntreeOptions = await _menuService.GetOptionsByEntree(customSandwichEntree.Id);

            BreadOptions = AllEntreeOptions.Where(o => o.FoodOptionName.Contains("Bread") ||
                                                       o.FoodOptionName.Contains("Bun") ||
                                                       o.FoodOptionName.Contains("Rye") ||
                                                       o.FoodOptionName.Contains("Sourdough") ||
                                                       o.FoodOptionName.Contains("Wheat") ||
                                                       o.FoodOptionName.Contains("Pita"))
                                          .Select(o => o.FoodOptionName).ToList();

            MeatOptions = AllEntreeOptions.Where(o => o.FoodOptionName.Contains("Ham") ||
                                                     o.FoodOptionName.Contains("Turkey") ||
                                                     o.FoodOptionName.Contains("Bacon") ||
                                                     o.FoodOptionName.Contains("Chicken") ||
                                                     o.FoodOptionName.Contains("Tuna"))
                                         .Select(o => o.FoodOptionName).ToList();

            CheeseOptions = AllEntreeOptions.Where(o => o.FoodOptionName.Contains("Cheese") ||
                                                       o.FoodOptionName.Contains("American") ||
                                                       o.FoodOptionName.Contains("Provolone") ||
                                                       o.FoodOptionName.Contains("Cheddar") ||
                                                       o.FoodOptionName.Contains("Swiss") ||
                                                       o.FoodOptionName.Contains("Mozzarella"))
                                           .Select(o => o.FoodOptionName).ToList();

            ToppingOptions = AllEntreeOptions.Where(o => (o.FoodOptionName.Contains("Lettuce") ||
                                                        o.FoodOptionName.Contains("Spinach") ||
                                                        o.FoodOptionName.Contains("Olive") ||
                                                        o.FoodOptionName.Contains("Cucumber") ||
                                                        o.FoodOptionName.Contains("Tomato") ||
                                                        o.FoodOptionName.Contains("Sprout") ||
                                                        o.FoodOptionName.Contains("Pepper") ||
                                                        o.FoodOptionName.Contains("Onion") ||
                                                        o.FoodOptionName.Contains("Pickle")) &&
                                                        !o.FoodOptionName.Contains("Pepper Jack"))
                                            .Select(o => o.FoodOptionName).ToList();

            DressingOptions = AllEntreeOptions.Where(o => o.FoodOptionName.Contains("Oil") ||
                                                         o.FoodOptionName.Contains("Vinegar") ||
                                                         o.FoodOptionName.Contains("Ranch") ||
                                                         o.FoodOptionName.Contains("Island") ||
                                                         o.FoodOptionName.Contains("Italian") ||
                                                         o.FoodOptionName.Contains("Caesar") ||
                                                         o.FoodOptionName.Contains("Vinaigrette") ||
                                                         o.FoodOptionName.Contains("Mustard") ||
                                                         o.FoodOptionName.Contains("Mayo"))
                                             .Select(o => o.FoodOptionName).ToList();

            if (!BreadOptions.Any())
            {
                BreadOptions = new() { "Pretzel Bun", "Marble Rye", "Sourdough", "Pita Bread", "Wheat", "White Bread" };
                MeatOptions = new() { "Ham", "Turkey", "Bacon", "Grilled Chicken", "Tuna Salad" };
                CheeseOptions = new() { "American", "Provolone", "Cheddar", "Pepper Jack", "Swiss", "Mozzarella" };
                ToppingOptions = new() { "Spinach", "Romaine", "Green Leaf Lettuce", "Olives", "Cucumber", "Tomato", "Sprouts", "Bell Pepper", "Onions", "Pickles", "Banana Peppers" };
                DressingOptions = new() { "Oil", "Vinegar", "Ranch", "1000 Island", "Italian", "Caesar", "Raspberry Vinaigrette", "Honey Mustard", "Mayonnaise", "Mustard" };
            }
        }
    }

    public void SetActiveTab(string tab)
    {
        ActiveTab = tab;
    }

    public void SelectSide(SideDto side)
    {
        SelectedSide = side;
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

    public void SetBread(string bread)
    {
        SelectedBread = bread;
    }

    public void SetMeat(string meat)
    {
        SelectedMeat = meat;
    }

    public void SetCheese(string cheese)
    {
        SelectedCheese = cheese;
    }

    public void SetDressing(string dressing)
    {
        SelectedDressing = dressing;
    }

    public int GetSelectionCount()
    {
        int count = 0;
        if (SelectedBread != null) count++;
        if (SelectedMeat != null) count++;
        if (SelectedCheese != null) count++;
        if (SelectedToppings.Any()) count++;
        if (SelectedDressing != null) count++;
        if (SelectedSide != null) count++;
        if (SelectedDrink != null) count++;
        return count;
    }

    public string GetSelectionSummary()
    {
        if (!IsValidSelection())
        {
            return "Complete all required fields";
        }
        return $"{SelectedBread}, {SelectedMeat}, {SelectedCheese}, {SelectedToppings.Count} topping(s), {SelectedDressing}";
    }

    public bool IsValidSelection()
    {
        return SelectedBread != null
            && SelectedMeat != null
            && SelectedCheese != null
            && SelectedToppings.Any()
            && SelectedDressing != null
            && SelectedSide != null
            && SelectedDrink != null;
    }

    public async Task<bool> AddToOrderAsync()
    {
        if (!IsValidSelection() || SelectedSide == null || SelectedDrink == null)
            return false;

        var customSandwichEntree = Entrees.FirstOrDefault(e => e.EntreeName.Contains("Sandwich") || e.EntreeName.Contains("Deli"));
        if (customSandwichEntree == null)
        {
            customSandwichEntree = new EntreeDto
            {
                Id = 0,
                StationId = StationId,
                EntreeName = "Custom Deli Sandwich",
                EntreePrice = 6.99m
            };
        }

        await _cartService.AddEntree(CART_KEY, customSandwichEntree);

        if (!string.IsNullOrEmpty(SelectedBread))
        {
            var breadOption = AllEntreeOptions.FirstOrDefault(o => o.FoodOptionName == SelectedBread);
            if (breadOption != null)
            {
                var optionType = new FoodOptionTypeDto { FoodOptionTypeName = "Bread Choice", EntreeId = customSandwichEntree.Id };
                await _cartService.AddEntreeOption(CART_KEY, customSandwichEntree.Id, breadOption, optionType);
            }
        }

        if (!string.IsNullOrEmpty(SelectedMeat))
        {
            var meatOption = AllEntreeOptions.FirstOrDefault(o => o.FoodOptionName == SelectedMeat);
            if (meatOption != null)
            {
                var optionType = new FoodOptionTypeDto { FoodOptionTypeName = "Meat Choice", EntreeId = customSandwichEntree.Id };
                await _cartService.AddEntreeOption(CART_KEY, customSandwichEntree.Id, meatOption, optionType);
            }
        }

        if (!string.IsNullOrEmpty(SelectedCheese))
        {
            var cheeseOption = AllEntreeOptions.FirstOrDefault(o => o.FoodOptionName == SelectedCheese);
            if (cheeseOption != null)
            {
                var optionType = new FoodOptionTypeDto { FoodOptionTypeName = "Cheese Choice", EntreeId = customSandwichEntree.Id };
                await _cartService.AddEntreeOption(CART_KEY, customSandwichEntree.Id, cheeseOption, optionType);
            }
        }

        foreach (var topping in SelectedToppings)
        {
            var toppingOption = AllEntreeOptions.FirstOrDefault(o => o.FoodOptionName == topping);
            if (toppingOption != null)
            {
                var optionType = new FoodOptionTypeDto { FoodOptionTypeName = "Toppings", EntreeId = customSandwichEntree.Id };
                await _cartService.AddEntreeOption(CART_KEY, customSandwichEntree.Id, toppingOption, optionType);
            }
        }

        if (!string.IsNullOrEmpty(SelectedDressing))
        {
            var dressingOption = AllEntreeOptions.FirstOrDefault(o => o.FoodOptionName == SelectedDressing);
            if (dressingOption != null)
            {
                var optionType = new FoodOptionTypeDto { FoodOptionTypeName = "Dressing Choice", EntreeId = customSandwichEntree.Id };
                await _cartService.AddEntreeOption(CART_KEY, customSandwichEntree.Id, dressingOption, optionType);
            }
        }

        await _cartService.AddSide(CART_KEY, SelectedSide);

        await _cartService.AddDrink(CART_KEY, SelectedDrink);

        ClearSelections();
        return true;
    }

    private void ClearSelections()
    {
        SelectedBread = null;
        SelectedMeat = null;
        SelectedCheese = null;
        SelectedToppings.Clear();
        SelectedDressing = null;
        SelectedSide = null;
        SelectedDrink = null;
        ActiveTab = "sandwich";
    }
}
