using Cafeteria.Customer.Components.Pages.Stations.Domain;
using Cafeteria.Customer.Services.Menu;
using Cafeteria.Shared.DTOs.Menu;

namespace Cafeteria.Customer.Components.Pages.Stations.Strategies;

public class OptionBuilderSelectionStrategy : BaseSelectionStrategy
{
    private readonly Func<EntreeDto, bool> _entreePredicate;
    private readonly string _virtualEntreeName;

    private EntreeDto? _builderEntree;

    public OptionBuilderSelectionStrategy(
        CartSubmitter cartSubmitter,
        IApiMenuService menuService,
        Func<EntreeDto, bool> entreePredicate,
        string virtualEntreeName)
        : base(cartSubmitter, menuService)
    {
        _entreePredicate = entreePredicate;
        _virtualEntreeName = virtualEntreeName;
    }

    public override bool IsValidSelection(SelectionState state, bool isCardOrder) =>
        SelectionValidator.IsValid(
            state, OptionTypes, isCardOrder,
            requiresOptionsComplete: true);

    public override int GetSelectionCount(SelectionState state)
    {
        int count = 0;
        foreach (var optionType in OptionTypes)
        {
            if (state.MultiSelectOptions.TryGetValue(optionType.OptionType.Id, out var selected) && selected.Any())
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
        var matched = entrees.FirstOrDefault(_entreePredicate);
        _builderEntree = matched ?? new EntreeDto
        {
            Id = 0,
            StationId = StationId,
            EntreeName = _virtualEntreeName,
            EntreePrice = 6.99m
        };
        state.SelectedEntree = _builderEntree;

        if (matched != null)
        {
            AllEntreeOptions = await MenuService.GetOptionsByEntree(matched.Id);
            OptionTypes = await MenuService.GetOptionTypesWithOptionsByEntree(matched.Id);
        }
    }

    public override void SetOptionForType(int optionTypeId, string optionName, SelectionState state)
    {
        state.MultiSelectOptions[optionTypeId] = new List<string> { optionName };
    }

    public override void ToggleOptionForType(int optionTypeId, string optionName, SelectionState state)
    {
        if (!state.MultiSelectOptions.ContainsKey(optionTypeId))
            state.MultiSelectOptions[optionTypeId] = new List<string>();

        var selected = state.MultiSelectOptions[optionTypeId];
        if (selected.Contains(optionName))
            selected.Remove(optionName);
        else
            selected.Add(optionName);
    }

    public override async Task AddToCartAsync(SelectionState state, bool isCardOrder)
    {
        if (!IsValidSelection(state, isCardOrder))
            return;

        await CartSubmitter.SubmitAsync(state, OptionTypes, AllEntreeOptions);
        ClearSelections(state, Entrees);
    }

    public override void ClearSelections(SelectionState state, List<EntreeDto> entrees)
    {
        state.Clear();
        state.SelectedEntree = _builderEntree;
    }

    public override string GetSelectionSummary(SelectionState state)
    {
        if (!IsValidSelection(state, true))
            return "Complete all required fields";

        var parts = new List<string>();
        foreach (var optionType in OptionTypes)
        {
            if (!state.MultiSelectOptions.TryGetValue(optionType.OptionType.Id, out var selected) || !selected.Any())
                continue;

            if (optionType.OptionType.FoodOptionTypeName is "Toppings" or "Fillings")
                parts.Add($"{selected.Count} {optionType.OptionType.FoodOptionTypeName.ToLower()}");
            else
                parts.Add(selected.First());
        }

        return parts.Any() ? string.Join(", ", parts) : _virtualEntreeName;
    }

    public override decimal GetExtraToppingCharge(SelectionState state)
    {
        decimal totalCharge = 0;
        foreach (var optionType in OptionTypes)
        {
            if (!state.MultiSelectOptions.TryGetValue(optionType.OptionType.Id, out var selected))
                continue;
            var extraCount = Math.Max(0, selected.Count - optionType.OptionType.NumIncluded);
            totalCharge += extraCount * optionType.OptionType.FoodOptionPrice;
        }
        return totalCharge;
    }

    public override bool HasExtraToppingCharge(SelectionState state)
    {
        foreach (var optionType in OptionTypes)
        {
            if (!state.MultiSelectOptions.TryGetValue(optionType.OptionType.Id, out var selected))
                continue;
            if (selected.Count > optionType.OptionType.NumIncluded && optionType.OptionType.FoodOptionPrice > 0)
                return true;
        }
        return false;
    }
}
