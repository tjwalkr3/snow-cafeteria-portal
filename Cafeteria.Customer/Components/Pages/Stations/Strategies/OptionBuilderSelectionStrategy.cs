using Cafeteria.Customer.Components.Pages.Stations.Configuration;
using Cafeteria.Customer.Components.Pages.Stations.Models;
using Cafeteria.Customer.Services.Cart;
using Cafeteria.Customer.Services.Menu;
using Cafeteria.Shared.DTOs.Menu;

namespace Cafeteria.Customer.Components.Pages.Stations.Strategies;

public class OptionBuilderSelectionStrategy : BaseSelectionStrategy
{
    private readonly Func<EntreeDto, bool> _entreePredicate;
    private readonly string _virtualEntreeName;

    public override StationType StationType { get; }

    public OptionBuilderSelectionStrategy(
        ICartService cartService,
        IApiMenuService menuService,
        StationType stationType,
        Func<EntreeDto, bool> entreePredicate,
        string virtualEntreeName)
        : base(cartService, menuService)
    {
        StationType = stationType;
        _entreePredicate = entreePredicate;
        _virtualEntreeName = virtualEntreeName;
    }

    public override bool IsValidSelection(SelectionState state, bool isCardOrder)
    {
        if (isCardOrder)
        {
            if (IsBuilderComplete(state))
                return true;
            return state.SelectedSide != null || state.SelectedDrink != null;
        }

        if (state.SelectedSide == null || state.SelectedDrink == null)
            return false;

        return IsBuilderComplete(state);
    }

    private bool IsBuilderComplete(SelectionState state)
    {
        if (!OptionTypes.Any())
            return false;

        foreach (var optionType in OptionTypes)
        {
            var selected = state.MultiSelectOptions.TryGetValue(optionType.OptionType.Id, out var value)
                ? value
                : new List<string>();

            if (selected.Count < optionType.OptionType.NumIncluded)
                return false;
        }

        return true;
    }

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
        var builderEntree = entrees.FirstOrDefault(_entreePredicate);
        if (builderEntree != null)
        {
            AllEntreeOptions = await MenuService.GetOptionsByEntree(builderEntree.Id);
            OptionTypes = await MenuService.GetOptionTypesWithOptionsByEntree(builderEntree.Id);
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

        if (isCardOrder)
        {
            if (IsBuilderComplete(state))
                await AddBuilderEntreeToCart(state);
            if (state.SelectedSide != null)
                await CartService.AddSide(CART_KEY, state.SelectedSide);
            if (state.SelectedDrink != null)
                await CartService.AddDrink(CART_KEY, state.SelectedDrink);
        }
        else
        {
            await AddBuilderEntreeToCart(state);
            await CartService.AddSide(CART_KEY, state.SelectedSide!);
            await CartService.AddDrink(CART_KEY, state.SelectedDrink!);
        }

        ClearSelections(state, Entrees);
    }

    private async Task AddBuilderEntreeToCart(SelectionState state)
    {
        var entree = Entrees.FirstOrDefault(_entreePredicate) ?? new EntreeDto
        {
            Id = 0,
            StationId = StationId,
            EntreeName = _virtualEntreeName,
            EntreePrice = 6.99m
        };

        await CartService.AddEntree(CART_KEY, entree);

        foreach (var optionType in OptionTypes)
        {
            if (!state.MultiSelectOptions.TryGetValue(optionType.OptionType.Id, out var selectedOptions))
                continue;

            foreach (var selectedOptionName in selectedOptions)
            {
                var option = optionType.Options.FirstOrDefault(o => o.FoodOptionName == selectedOptionName);
                if (option != null)
                    await CartService.AddEntreeOption(CART_KEY, entree.Id, option, optionType.OptionType);
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
