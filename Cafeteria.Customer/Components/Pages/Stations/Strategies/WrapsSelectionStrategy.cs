using Cafeteria.Customer.Components.Pages.Stations.Configuration;
using Cafeteria.Customer.Components.Pages.Stations.Models;
using Cafeteria.Customer.Services.Cart;
using Cafeteria.Customer.Services.Menu;
using Cafeteria.Shared.DTOs.Menu;

namespace Cafeteria.Customer.Components.Pages.Stations.Strategies;

public class WrapsSelectionStrategy : BaseSelectionStrategy
{
    public override StationType StationType => StationType.Wraps;

    public WrapsSelectionStrategy(ICartService cartService, IApiMenuService menuService)
        : base(cartService, menuService)
    {
    }

    public override bool IsValidSelection(SelectionState state, bool isCardOrder)
    {
        if (isCardOrder)
        {
            // Card orders: allow complete wrap, or just side, or just drink
            if (IsWrapComplete(state))
                return true;
            return state.SelectedSide != null || state.SelectedDrink != null;
        }

        // Swipe orders: require complete wrap + side + drink
        if (state.SelectedSide == null || state.SelectedDrink == null)
            return false;

        return IsWrapComplete(state);
    }

    private bool IsWrapComplete(SelectionState state)
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

        var wrapEntree = entrees.FirstOrDefault(e =>
            e.EntreeName.Contains("Wrap", StringComparison.OrdinalIgnoreCase));

        if (wrapEntree != null)
        {
            AllEntreeOptions = await MenuService.GetOptionsByEntree(wrapEntree.Id);
            OptionTypes = await MenuService.GetOptionTypesWithOptionsByEntree(wrapEntree.Id);
        }
    }

    public override void SetOptionForType(int optionTypeId, string optionName, SelectionState state)
    {
        // For Wraps, single-select options are stored in MultiSelectOptions with a single item
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
            // Add only selected items
            if (IsWrapComplete(state))
            {
                await AddWrapToCart(state);
            }
            if (state.SelectedSide != null)
                await CartService.AddSide(CART_KEY, state.SelectedSide);
            if (state.SelectedDrink != null)
                await CartService.AddDrink(CART_KEY, state.SelectedDrink);
        }
        else
        {
            // Swipe: add all three
            await AddWrapToCart(state);
            await CartService.AddSide(CART_KEY, state.SelectedSide!);
            await CartService.AddDrink(CART_KEY, state.SelectedDrink!);
        }

        ClearSelections(state, Entrees);
    }

    private async Task AddWrapToCart(SelectionState state)
    {
        var wrapEntree = Entrees.FirstOrDefault(e =>
            e.EntreeName.Contains("Wrap", StringComparison.OrdinalIgnoreCase));

        if (wrapEntree == null)
        {
            wrapEntree = new EntreeDto
            {
                Id = 0,
                StationId = StationId,
                EntreeName = "Custom Wrap",
                EntreePrice = 6.99m
            };
        }

        await CartService.AddEntree(CART_KEY, wrapEntree);

        foreach (var optionType in OptionTypes)
        {
            var selectedOptions = GetSelectedOptionsForType(optionType.OptionType.Id, state);

            foreach (var selectedOptionName in selectedOptions)
            {
                var option = optionType.Options.FirstOrDefault(o => o.FoodOptionName == selectedOptionName);
                if (option != null)
                {
                    await CartService.AddEntreeOption(CART_KEY, wrapEntree.Id, option, optionType.OptionType);
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
                if (optionType.OptionType.FoodOptionTypeName == "Toppings" ||
                    optionType.OptionType.FoodOptionTypeName == "Fillings")
                {
                    parts.Add($"{selected.Count} filling(s)");
                }
                else
                {
                    parts.Add(selected.First());
                }
            }
        }

        return parts.Any() ? string.Join(", ", parts) : "Custom Wrap";
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
               optionType.OptionType.FoodOptionTypeName == "Toppings" ||
               optionType.OptionType.FoodOptionTypeName == "Fillings";
    }
}
