using Cafeteria.Customer.Components.Pages.Stations.Configuration;
using Cafeteria.Customer.Components.Pages.Stations.Domain;
using Cafeteria.Customer.Services.Menu;
using Cafeteria.Shared.DTOs.Menu;

namespace Cafeteria.Customer.Components.Pages.Stations.Strategies;

public class PizzaSelectionStrategy : BaseSelectionStrategy
{
    private const int MINIMUM_TOPPINGS = 2;

    private static readonly List<string> FallbackToppings = new()
    {
        "Extra Cheese", "Pepperoni", "Sausage", "Bacon", "Chicken", "Ham",
        "Olives", "Mushrooms", "Onions", "Pineapple", "Bell Peppers", "Banana Peppers"
    };

    public override StationType StationType => StationType.Pizza;

    public PizzaSelectionStrategy(CartSubmitter cartSubmitter, IApiMenuService menuService)
        : base(cartSubmitter, menuService)
    {
    }

    public override bool IsValidSelection(SelectionState state, bool isCardOrder) =>
        SelectionValidator.IsValid(
            state, OptionTypes, isCardOrder,
            requiresOptionsComplete: false,
            minimumToppings: OptionTypes.Any()
                ? OptionTypes.First().OptionType.NumIncluded
                : MINIMUM_TOPPINGS);

    public override int GetSelectionCount(SelectionState state)
    {
        int count = 0;
        if (state.SelectedToppings.Count >= MINIMUM_TOPPINGS) count++;
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

        var pizzaEntree = entrees.FirstOrDefault();
        if (pizzaEntree != null)
        {
            AllEntreeOptions = await MenuService.GetOptionsByEntree(pizzaEntree.Id);
            AvailableToppings = AllEntreeOptions.Select(o => o.FoodOptionName).ToList();

            if (!AvailableToppings.Any())
                AvailableToppings = new List<string>(FallbackToppings);

            OptionTypes = await MenuService.GetOptionTypesWithOptionsByEntree(pizzaEntree.Id);
        }

        if (entrees.Any())
            state.SelectedEntree = entrees.First();
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
        state.SelectedSide = null;
        state.SelectedDrink = null;
        state.SelectedToppings.Clear();

        if (entrees.Any())
            state.SelectedEntree = entrees.First();
    }

    public override string GetSelectionSummary(SelectionState state)
    {
        if (!IsValidSelection(state, true))
            return "Complete all required fields";

        return $"Personal Pizza with {state.SelectedToppings.Count} topping(s)";
    }

    public override decimal GetExtraToppingCharge(SelectionState state)
    {
        var toppingsOptionType = OptionTypes.FirstOrDefault();
        if (toppingsOptionType == null)
            return 0m;

        int extraToppings = Math.Max(0, state.SelectedToppings.Count - toppingsOptionType.OptionType.NumIncluded);
        return extraToppings * toppingsOptionType.OptionType.FoodOptionPrice;
    }

    public override bool HasExtraToppingCharge(SelectionState state)
    {
        var toppingsOptionType = OptionTypes.FirstOrDefault();
        if (toppingsOptionType == null)
            return false;

        return state.SelectedToppings.Count > toppingsOptionType.OptionType.NumIncluded;
    }
}
