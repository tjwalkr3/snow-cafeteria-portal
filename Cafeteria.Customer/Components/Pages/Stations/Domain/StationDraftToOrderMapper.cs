using Cafeteria.Shared.DTOs.Menu;
using Cafeteria.Shared.DTOs.Order;

namespace Cafeteria.Customer.Components.Pages.Stations.Domain;

public sealed class StationOrderItems
{
    public List<OrderEntreeItem> Entrees { get; } = [];
    public List<OrderSideItem> Sides { get; } = [];
    public List<DrinkDto> Drinks { get; } = [];
}

public sealed class CardStationDraft
{
    public IReadOnlyList<EntreeDto> Entrees { get; init; } = [];
    public IReadOnlyList<SideWithOptionsDto> Sides { get; init; } = [];
    public IReadOnlyList<DrinkDto> Drinks { get; init; } = [];

    public IReadOnlyDictionary<int, int> EntreeQuantities { get; init; } = new Dictionary<int, int>();
    public IReadOnlyDictionary<int, int> SideQuantities { get; init; } = new Dictionary<int, int>();
    public IReadOnlyDictionary<int, int> DrinkQuantities { get; init; } = new Dictionary<int, int>();

    public IReadOnlyDictionary<int, List<FoodOptionTypeWithOptionsDto>> EntreeOptionTypes { get; init; } =
        new Dictionary<int, List<FoodOptionTypeWithOptionsDto>>();

    public IReadOnlyDictionary<int, List<FoodOptionTypeWithOptionsDto>> SideOptionTypes { get; init; } =
        new Dictionary<int, List<FoodOptionTypeWithOptionsDto>>();

    // itemId -> optionTypeId -> selected option names
    public IReadOnlyDictionary<int, Dictionary<int, HashSet<string>>> EntreeOptions { get; init; } =
        new Dictionary<int, Dictionary<int, HashSet<string>>>();

    public IReadOnlyDictionary<int, Dictionary<int, HashSet<string>>> SideOptions { get; init; } =
        new Dictionary<int, Dictionary<int, HashSet<string>>>();
}

public static class StationDraftToOrderMapper
{
    public static StationOrderItems MapCardSelections(CardStationDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);

        var result = new StationOrderItems();

        foreach (var entree in draft.Entrees)
        {
            if (!draft.EntreeQuantities.TryGetValue(entree.Id, out var qty) || qty <= 0)
            {
                continue;
            }

            var selectedOptions = BuildItemOptions(
                entree.Id,
                draft.EntreeOptionTypes,
                draft.EntreeOptions);

            for (var i = 0; i < qty; i++)
            {
                result.Entrees.Add(new OrderEntreeItem
                {
                    Entree = entree,
                    SelectedOptions = [.. selectedOptions]
                });
            }
        }

        foreach (var sideWithOptions in draft.Sides)
        {
            if (!draft.SideQuantities.TryGetValue(sideWithOptions.Side.Id, out var qty) || qty <= 0)
            {
                continue;
            }

            var selectedOptions = BuildItemOptions(
                sideWithOptions.Side.Id,
                draft.SideOptionTypes,
                draft.SideOptions,
                sideWithOptions.OptionTypes);

            for (var i = 0; i < qty; i++)
            {
                result.Sides.Add(new OrderSideItem
                {
                    Side = sideWithOptions.Side,
                    SelectedOptions = [.. selectedOptions]
                });
            }
        }

        foreach (var drink in draft.Drinks)
        {
            if (!draft.DrinkQuantities.TryGetValue(drink.Id, out var qty) || qty <= 0)
            {
                continue;
            }

            for (var i = 0; i < qty; i++)
            {
                result.Drinks.Add(drink);
            }
        }

        return result;
    }

    private static List<SelectedFoodOption> BuildItemOptions(
        int itemId,
        IReadOnlyDictionary<int, List<FoodOptionTypeWithOptionsDto>> optionTypesByItem,
        IReadOnlyDictionary<int, Dictionary<int, HashSet<string>>> selectionsByItem,
        IReadOnlyList<FoodOptionTypeWithOptionsDto>? fallbackOptionTypes = null)
    {
        optionTypesByItem.TryGetValue(itemId, out var mappedOptionTypes);
        IReadOnlyList<FoodOptionTypeWithOptionsDto>? optionTypes = mappedOptionTypes ?? fallbackOptionTypes;

        if (optionTypes == null || optionTypes.Count == 0)
        {
            return [];
        }

        if (!selectionsByItem.TryGetValue(itemId, out var selectedNamesByOptionType))
        {
            return [];
        }

        return ResolveSelectedOptions(optionTypes, selectedNamesByOptionType);
    }

    private static List<SelectedFoodOption> ResolveSelectedOptions(
        IReadOnlyList<FoodOptionTypeWithOptionsDto> optionTypes,
        IReadOnlyDictionary<int, HashSet<string>> selectedNamesByOptionType)
    {
        var selected = new List<SelectedFoodOption>();

        foreach (var optionTypeWithOptions in optionTypes)
        {
            var optionTypeId = optionTypeWithOptions.OptionType.Id;
            if (!selectedNamesByOptionType.TryGetValue(optionTypeId, out var names) || names.Count == 0)
            {
                continue;
            }

            foreach (var option in optionTypeWithOptions.Options)
            {
                if (names.Contains(option.FoodOptionName))
                {
                    selected.Add(new SelectedFoodOption { Option = option, OptionType = optionTypeWithOptions.OptionType });
                }
            }
        }

        return selected;
    }
}