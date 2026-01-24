using Cafeteria.Customer.Components.Pages.Stations.Configuration;
using Cafeteria.Customer.Components.Pages.Stations.Models;
using Cafeteria.Customer.Services;
using Cafeteria.Shared.DTOs;

namespace Cafeteria.Customer.Components.Pages.Stations.Strategies;

public class BreakfastSelectionStrategy : BaseSelectionStrategy
{
    public override StationType StationType => StationType.Breakfast;

    public BreakfastSelectionStrategy(ICartService cartService, IApiMenuService menuService)
        : base(cartService, menuService)
    {
    }

    public override bool IsValidSelection(SelectionState state, bool isCardOrder)
    {
        if (isCardOrder)
        {
            // Card orders: allow any selection
            // If entree is selected, ensure all required options are selected
            if (state.SelectedEntree != null)
            {
                foreach (var optionType in OptionTypes)
                {
                    if (!state.SingleSelectOptions.ContainsKey(optionType.OptionType.Id) ||
                        string.IsNullOrEmpty(state.SingleSelectOptions[optionType.OptionType.Id]))
                    {
                        return false;
                    }
                }
                return true;
            }
            // Allow just side or just drink
            return state.SelectedSide != null || state.SelectedDrink != null;
        }

        // Swipe orders: require all three
        if (state.SelectedEntree == null || state.SelectedSide == null || state.SelectedDrink == null)
            return false;

        // Validate all options are selected
        foreach (var optionType in OptionTypes)
        {
            if (!state.SingleSelectOptions.ContainsKey(optionType.OptionType.Id) ||
                string.IsNullOrEmpty(state.SingleSelectOptions[optionType.OptionType.Id]))
            {
                return false;
            }
        }

        return true;
    }

    public override async Task OnEntreeSelectedAsync(EntreeDto entree, SelectionState state)
    {
        state.SelectedEntree = entree;
        state.SingleSelectOptions.Clear();

        // Load options for the selected entree
        AllEntreeOptions = await MenuService.GetOptionsByEntree(entree.Id);
        OptionTypes = await MenuService.GetOptionTypesWithOptionsByEntree(entree.Id);
    }

    public override async Task AddToCartAsync(SelectionState state, bool isCardOrder)
    {
        if (!IsValidSelection(state, isCardOrder))
            return;

        if (isCardOrder)
        {
            // Add only selected items
            if (state.SelectedEntree != null)
            {
                await CartService.AddEntree(CART_KEY, state.SelectedEntree);

                foreach (var optionType in OptionTypes)
                {
                    if (state.SingleSelectOptions.TryGetValue(optionType.OptionType.Id, out var selectedOptionName))
                    {
                        var option = optionType.Options.FirstOrDefault(o => o.FoodOptionName == selectedOptionName);
                        if (option != null)
                        {
                            await CartService.AddEntreeOption(CART_KEY, state.SelectedEntree.Id, option, optionType.OptionType);
                        }
                    }
                }
            }
            if (state.SelectedSide != null)
                await CartService.AddSide(CART_KEY, state.SelectedSide);
            if (state.SelectedDrink != null)
                await CartService.AddDrink(CART_KEY, state.SelectedDrink);
        }
        else
        {
            // Swipe: add all three
            await CartService.AddEntree(CART_KEY, state.SelectedEntree!);

            foreach (var optionType in OptionTypes)
            {
                if (state.SingleSelectOptions.TryGetValue(optionType.OptionType.Id, out var selectedOptionName))
                {
                    var option = optionType.Options.FirstOrDefault(o => o.FoodOptionName == selectedOptionName);
                    if (option != null)
                    {
                        await CartService.AddEntreeOption(CART_KEY, state.SelectedEntree!.Id, option, optionType.OptionType);
                    }
                }
            }

            await CartService.AddSide(CART_KEY, state.SelectedSide!);
            await CartService.AddDrink(CART_KEY, state.SelectedDrink!);
        }

        ClearSelections(state, Entrees);
    }

    public override void ClearSelections(SelectionState state, List<EntreeDto> entrees)
    {
        state.Clear();
        OptionTypes.Clear();
        AllEntreeOptions.Clear();
    }

    public override string GetSelectionSummary(SelectionState state)
    {
        if (!IsValidSelection(state, true))
        {
            return "Complete all required fields";
        }

        var items = new List<string>();
        if (state.SelectedEntree != null) items.Add(state.SelectedEntree.EntreeName);
        if (state.SelectedSide != null) items.Add(state.SelectedSide.SideName);
        if (state.SelectedDrink != null) items.Add(state.SelectedDrink.DrinkName);

        return string.Join(", ", items);
    }
}
