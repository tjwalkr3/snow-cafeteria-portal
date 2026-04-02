using Cafeteria.Shared.DTOs.Menu;

namespace Cafeteria.Customer.Components.Pages.Stations.Domain;

public static class SelectionValidator
{
    public static bool IsValid(
        SelectionState state,
        List<FoodOptionTypeWithOptionsDto> optionTypes,
        bool isCardOrder,
        bool hasSides = true)
    {
        bool primaryComplete = IsPrimaryItemComplete(state, optionTypes);
        bool primaryStarted = state.SelectedEntree != null;

        if (isCardOrder)
        {
            if (primaryStarted && !primaryComplete)
                return false;

            return primaryComplete || state.SelectedSide != null || state.SelectedDrink != null;
        }

        return primaryComplete && (!hasSides || state.SelectedSide != null) && state.SelectedDrink != null;
    }

    public static bool AreOptionsComplete(
        SelectionState state,
        List<FoodOptionTypeWithOptionsDto> optionTypes)
    {
        foreach (var optionType in optionTypes)
        {
            var required = optionType.OptionType.RequiredAmount;
            if (required == 0)
                continue;

            if (optionType.OptionType.MaxAmount > 1)
            {
                var selected = state.MultiSelectOptions.TryGetValue(optionType.OptionType.Id, out var list)
                    ? list
                    : new List<string>();

                if (selected.Count < required)
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
        List<FoodOptionTypeWithOptionsDto> optionTypes)
    {
        if (state.SelectedEntree == null) return false;

        if (optionTypes.Any())
            return AreOptionsComplete(state, optionTypes);

        return true;
    }
}
