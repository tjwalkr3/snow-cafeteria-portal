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
    public List<FoodOptionTypeWithOptionsDto> OptionTypes { get; private set; } = new();

    public string ActiveTab { get; private set; } = "sandwich";
    public SideDto? SelectedSide { get; private set; }
    public DrinkDto? SelectedDrink { get; private set; }

    public Dictionary<int, List<string>> SelectedOptionsByType { get; private set; } = new();

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
            OptionTypes = await _menuService.GetOptionTypesWithOptionsByEntree(customSandwichEntree.Id);

            // Populate legacy option lists for backward compatibility
            PopulateLegacyOptions();
        }
    }

    private void PopulateLegacyOptions()
    {
        var breadType = OptionTypes.FirstOrDefault(ot => ot.OptionType.FoodOptionTypeName == "Bread");
        if (breadType != null)
            BreadOptions = breadType.Options.Select(o => o.FoodOptionName).ToList();

        var meatType = OptionTypes.FirstOrDefault(ot => ot.OptionType.FoodOptionTypeName == "Meat");
        if (meatType != null)
            MeatOptions = meatType.Options.Select(o => o.FoodOptionName).ToList();

        var cheeseType = OptionTypes.FirstOrDefault(ot => ot.OptionType.FoodOptionTypeName == "Cheese");
        if (cheeseType != null)
            CheeseOptions = cheeseType.Options.Select(o => o.FoodOptionName).ToList();

        var toppingsType = OptionTypes.FirstOrDefault(ot => ot.OptionType.FoodOptionTypeName == "Toppings");
        if (toppingsType != null)
            ToppingOptions = toppingsType.Options.Select(o => o.FoodOptionName).ToList();

        var dressingType = OptionTypes.FirstOrDefault(ot => ot.OptionType.FoodOptionTypeName == "Dressing");
        if (dressingType != null)
            DressingOptions = dressingType.Options.Select(o => o.FoodOptionName).ToList();
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

        var toppingsType = OptionTypes.FirstOrDefault(ot => ot.OptionType.FoodOptionTypeName == "Toppings");
        if (toppingsType != null)
        {
            SelectedOptionsByType[toppingsType.OptionType.Id] = new List<string>(SelectedToppings);
        }
    }

    public void ToggleOptionForType(int optionTypeId, string optionName)
    {
        if (!SelectedOptionsByType.ContainsKey(optionTypeId))
        {
            SelectedOptionsByType[optionTypeId] = new List<string>();
        }

        var selectedOptions = SelectedOptionsByType[optionTypeId];
        if (selectedOptions.Contains(optionName))
        {
            selectedOptions.Remove(optionName);
        }
        else
        {
            selectedOptions.Add(optionName);
        }
    }

    public void SetOptionForType(int optionTypeId, string optionName)
    {
        SelectedOptionsByType[optionTypeId] = new List<string> { optionName };

        var optionType = OptionTypes.FirstOrDefault(ot => ot.OptionType.Id == optionTypeId);
        if (optionType != null)
        {
            switch (optionType.OptionType.FoodOptionTypeName)
            {
                case "Bread":
                    SelectedBread = optionName;
                    break;
                case "Meat":
                    SelectedMeat = optionName;
                    break;
                case "Cheese":
                    SelectedCheese = optionName;
                    break;
                case "Dressing":
                    SelectedDressing = optionName;
                    break;
            }
        }
    }

    public List<string> GetSelectedOptionsForType(int optionTypeId)
    {
        return SelectedOptionsByType.TryGetValue(optionTypeId, out var value) ? value : new List<string>();
    }

    public bool IsOptionSelected(int optionTypeId, string optionName)
    {
        return GetSelectedOptionsForType(optionTypeId).Contains(optionName);
    }

    public void SetBread(string bread)
    {
        SelectedBread = bread;
        var breadType = OptionTypes.FirstOrDefault(ot => ot.OptionType.FoodOptionTypeName == "Bread");
        if (breadType != null)
        {
            SetOptionForType(breadType.OptionType.Id, bread);
        }
    }

    public void SetMeat(string meat)
    {
        SelectedMeat = meat;
        var meatType = OptionTypes.FirstOrDefault(ot => ot.OptionType.FoodOptionTypeName == "Meat");
        if (meatType != null)
        {
            SetOptionForType(meatType.OptionType.Id, meat);
        }
    }

    public void SetCheese(string cheese)
    {
        SelectedCheese = cheese;
        var cheeseType = OptionTypes.FirstOrDefault(ot => ot.OptionType.FoodOptionTypeName == "Cheese");
        if (cheeseType != null)
        {
            SetOptionForType(cheeseType.OptionType.Id, cheese);
        }
    }

    public void SetDressing(string dressing)
    {
        SelectedDressing = dressing;
        var dressingType = OptionTypes.FirstOrDefault(ot => ot.OptionType.FoodOptionTypeName == "Dressing");
        if (dressingType != null)
        {
            SetOptionForType(dressingType.OptionType.Id, dressing);
        }
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
        if (SelectedSide == null || SelectedDrink == null)
            return false;

        if (OptionTypes.Any())
        {
            foreach (var optionType in OptionTypes)
            {
                var selectedOptions = GetSelectedOptionsForType(optionType.OptionType.Id);

                if (selectedOptions.Count < optionType.OptionType.NumIncluded)
                {
                    return false;
                }
            }
            return true;
        }

        return SelectedBread != null
            && SelectedMeat != null
            && SelectedCheese != null
            && SelectedToppings.Any()
            && SelectedDressing != null;
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

        foreach (var optionType in OptionTypes)
        {
            var selectedOptions = GetSelectedOptionsForType(optionType.OptionType.Id);

            foreach (var selectedOptionName in selectedOptions)
            {
                var option = optionType.Options.FirstOrDefault(o => o.FoodOptionName == selectedOptionName);
                if (option != null)
                {
                    await _cartService.AddEntreeOption(CART_KEY, customSandwichEntree.Id, option, optionType.OptionType);
                }
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
        SelectedOptionsByType.Clear();
        ActiveTab = "sandwich";
    }
}
