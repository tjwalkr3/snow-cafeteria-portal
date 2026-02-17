using Cafeteria.Customer.Components.Pages.Stations.Configuration;
using Cafeteria.Customer.Components.Pages.Stations.Models;
using Cafeteria.Customer.Services;
using Cafeteria.Customer.Services.Cart;
using Cafeteria.Customer.Services.Menu;
using Cafeteria.Shared.DTOs.Menu;

namespace Cafeteria.Customer.Components.Pages.Stations.Strategies;

public class DeliSelectionStrategy : BaseSelectionStrategy
{
    public override StationType StationType => StationType.Deli;

    public DeliSelectionStrategy(ICartService cartService, IApiMenuService menuService)
        : base(cartService, menuService)
    {
    }

    public override bool IsValidSelection(SelectionState state, bool isCardOrder)
    {
        if (isCardOrder)
        {
            if (IsSandwichComplete(state))
                return true;
            return state.SelectedSide != null || state.SelectedDrink != null;
        }

        if (state.SelectedSide == null || state.SelectedDrink == null)
            return false;

        return IsSandwichComplete(state);
    }

    private bool IsSandwichComplete(SelectionState state)
    {
        if (OptionTypes.Any())
        {
            foreach (var optionType in OptionTypes)
            {
                var selectedOptions = GetSelectedOptionsForType(optionType.OptionType.Id, state);

                if (selectedOptions.Count < optionType.OptionType.NumIncluded)
                {
                    return false;
                }
            }
            return true;
        }

        return false;
    }

    private List<string> GetSelectedOptionsForType(int optionTypeId, SelectionState state)
    {
        return state.MultiSelectOptions.TryGetValue(optionTypeId, out var value) ? value : new List<string>();
    }

    public override int GetSelectionCount(SelectionState state)
    {
        int count = 0;
        foreach (var optionType in OptionTypes)
        {
            if (GetSelectedOptionsForType(optionType.OptionType.Id, state).Any())
                count++;
        }
        if (state.SelectedSide != null) count++;
        if (state.SelectedDrink != null) count++;
        return count;
    }

    public override async Task OnDataLoadedAsync(
        List<EntreeDto> entrees,
        List<SideDto> sides,
        List<DrinkDto> drinks,
        SelectionState state)
    {
        Entrees = entrees;

        var customSandwichEntree = entrees.FirstOrDefault(e =>
            e.EntreeName.Contains("Sandwich") || e.EntreeName.Contains("Deli"));

        if (customSandwichEntree != null)
        {
            AllEntreeOptions = await MenuService.GetOptionsByEntree(customSandwichEntree.Id);
            OptionTypes = await MenuService.GetOptionTypesWithOptionsByEntree(customSandwichEntree.Id);
        }
    }

    public override void SetOptionForType(int optionTypeId, string optionName, SelectionState state)
    {
        state.MultiSelectOptions[optionTypeId] = new List<string> { optionName };
    }

    public override void ToggleOptionForType(int optionTypeId, string optionName, SelectionState state)
    {
        if (!state.MultiSelectOptions.ContainsKey(optionTypeId))
        {
            state.MultiSelectOptions[optionTypeId] = new List<string>();
        }

        var selectedOptions = state.MultiSelectOptions[optionTypeId];
        if (selectedOptions.Contains(optionName))
        {
            selectedOptions.Remove(optionName);
        }
        else
        {
            selectedOptions.Add(optionName);
        }
    }

    public bool IsOptionSelected(int optionTypeId, string optionName, SelectionState state)
    {
        return GetSelectedOptionsForType(optionTypeId, state).Contains(optionName);
    }

    public override async Task AddToCartAsync(SelectionState state, bool isCardOrder)
    {
        if (!IsValidSelection(state, isCardOrder))
            return;

        if (isCardOrder)
        {
            if (IsSandwichComplete(state))
            {
                await AddSandwichToCart(state);
            }
            if (state.SelectedSide != null)
                await CartService.AddSide(CART_KEY, state.SelectedSide);
            if (state.SelectedDrink != null)
                await CartService.AddDrink(CART_KEY, state.SelectedDrink);
        }
        else
        {
            await AddSandwichToCart(state);
            await CartService.AddSide(CART_KEY, state.SelectedSide!);
            await CartService.AddDrink(CART_KEY, state.SelectedDrink!);
        }

        ClearSelections(state, Entrees);
    }

    private async Task AddSandwichToCart(SelectionState state)
    {
        var customSandwichEntree = Entrees.FirstOrDefault(e =>
            e.EntreeName.Contains("Sandwich") || e.EntreeName.Contains("Deli"));

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

        await CartService.AddEntree(CART_KEY, customSandwichEntree);

        foreach (var optionType in OptionTypes)
        {
            var selectedOptions = GetSelectedOptionsForType(optionType.OptionType.Id, state);

            foreach (var selectedOptionName in selectedOptions)
            {
                var option = optionType.Options.FirstOrDefault(o => o.FoodOptionName == selectedOptionName);
                if (option != null)
                {
                    await CartService.AddEntreeOption(CART_KEY, customSandwichEntree.Id, option, optionType.OptionType);
                }
            }
        }
    }

    public override void ClearSelections(SelectionState state, List<EntreeDto> entrees)
    {
        state.Clear();
    }

    public override string GetSelectionSummary(SelectionState state)
    {
        if (!IsValidSelection(state, true))
        {
            return "Complete all required fields";
        }

        var parts = new List<string>();
        foreach (var optionType in OptionTypes)
        {
            var selected = GetSelectedOptionsForType(optionType.OptionType.Id, state);
            if (selected.Any())
            {
                if (optionType.OptionType.FoodOptionTypeName == "Toppings")
                {
                    parts.Add($"{selected.Count} topping(s)");
                }
                else
                {
                    parts.Add(selected.First());
                }
            }
        }

        return parts.Any() ? string.Join(", ", parts) : "Custom Deli Sandwich";
    }

    public override decimal GetExtraToppingCharge(SelectionState state)
    {
        decimal totalCharge = 0;
        foreach (var optionType in OptionTypes)
        {
            var selectedCount = GetSelectedOptionsForType(optionType.OptionType.Id, state).Count;
            var extraCount = Math.Max(0, selectedCount - optionType.OptionType.NumIncluded);
            totalCharge += extraCount * optionType.OptionType.FoodOptionPrice;
        }
        return totalCharge;
    }

    public override bool HasExtraToppingCharge(SelectionState state)
    {
        foreach (var optionType in OptionTypes)
        {
            var selectedCount = GetSelectedOptionsForType(optionType.OptionType.Id, state).Count;
            if (selectedCount > optionType.OptionType.NumIncluded && optionType.OptionType.FoodOptionPrice > 0)
            {
                return true;
            }
        }
        return false;
    }

    public bool IsMultiSelectOptionType(FoodOptionTypeWithOptionsDto optionType)
    {
        return optionType.OptionType.MaxAmount > 1 ||
               optionType.OptionType.FoodOptionTypeName == "Toppings";
    }
}
