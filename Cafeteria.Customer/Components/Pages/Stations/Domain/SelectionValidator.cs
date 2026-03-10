using Cafeteria.Shared.DTOs.Menu;

namespace Cafeteria.Customer.Components.Pages.Stations.Domain;

public static class SelectionValidator
{
    public static bool IsValid(
        SelectionState state,
        List<FoodOptionTypeWithOptionsDto> optionTypes,
        bool isCardOrder,
        bool requiresOptionsComplete,
        int minimumToppings = 0)
    {
        bool primaryComplete = IsPrimaryItemComplete(state, optionTypes, requiresOptionsComplete, minimumToppings);
        bool primaryStarted = IsPrimaryItemStarted(state, minimumToppings);

        if (isCardOrder)
        {
            if (primaryStarted && !primaryComplete)
                return false;

            return primaryComplete || state.SelectedSide != null || state.SelectedDrink != null;
        }

        return primaryComplete && state.SelectedSide != null && state.SelectedDrink != null;
    }

    public static bool AreOptionsComplete(
        SelectionState state,
        List<FoodOptionTypeWithOptionsDto> optionTypes)
    {
        foreach (var optionType in optionTypes)
        {
            if (OptionTypeHelper.IsMultiSelectOptionType(optionType))
            {
                var selected = state.MultiSelectOptions.TryGetValue(optionType.OptionType.Id, out var list)
                    ? list
                    : new List<string>();

                if (selected.Count < optionType.OptionType.NumIncluded)
                    return false;
            }
            else
            {
                if (!state.SingleSelectOptions.TryGetValue(optionType.OptionType.Id, out var value)
                    || string.IsNullOrEmpty(value))
                    return false;
            }
        }

        return true;
    }

    private static bool IsPrimaryItemComplete(
        SelectionState state,
        List<FoodOptionTypeWithOptionsDto> optionTypes,
        bool requiresOptionsComplete,
        int minimumToppings)
    {
        if (minimumToppings > 0)
            return state.SelectedToppings.Count >= minimumToppings;

        if (state.SelectedEntree == null) return false;

        if (requiresOptionsComplete && optionTypes.Any())
            return AreOptionsComplete(state, optionTypes);

        return true;
    }

    private static bool IsPrimaryItemStarted(SelectionState state, int minimumToppings)
    {
        if (minimumToppings > 0)
            return state.SelectedToppings.Count > 0;

        return state.SelectedEntree != null;
    }
}
