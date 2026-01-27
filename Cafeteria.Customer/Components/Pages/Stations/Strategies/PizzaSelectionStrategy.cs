using Cafeteria.Customer.Components.Pages.Stations.Configuration;
using Cafeteria.Customer.Components.Pages.Stations.Models;
using Cafeteria.Customer.Services;
using Cafeteria.Shared.DTOs.Menu;

namespace Cafeteria.Customer.Components.Pages.Stations.Strategies;

public class PizzaSelectionStrategy : BaseSelectionStrategy
{
    private const int MINIMUM_TOPPINGS = 2;
    private const int INCLUDED_TOPPINGS = 2;
    private const decimal EXTRA_TOPPING_PRICE = 0.50m;

    private static readonly List<string> FallbackToppings = new()
    {
        "Extra Cheese", "Pepperoni", "Sausage", "Bacon", "Chicken", "Ham",
        "Olives", "Mushrooms", "Onions", "Pineapple", "Bell Peppers", "Banana Peppers"
    };

    public override StationType StationType => StationType.Pizza;

    public PizzaSelectionStrategy(ICartService cartService, IApiMenuService menuService)
        : base(cartService, menuService)
    {
    }

    public override bool IsValidSelection(SelectionState state, bool isCardOrder)
    {
        if (isCardOrder)
        {
            // Card orders: allow pizza with toppings, or just side, or just drink
            if (state.SelectedToppings.Count >= MINIMUM_TOPPINGS)
                return true;
            return state.SelectedSide != null || state.SelectedDrink != null;
        }

        // Swipe orders: require pizza with 2+ toppings + side + drink
        return state.SelectedToppings.Count >= MINIMUM_TOPPINGS &&
               state.SelectedSide != null &&
               state.SelectedDrink != null;
    }

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
            {
                AvailableToppings = new List<string>(FallbackToppings);
            }
        }

        // Auto-select first entree
        if (entrees.Any())
        {
            state.SelectedEntree = entrees.First();
        }
    }

    public override async Task AddToCartAsync(SelectionState state, bool isCardOrder)
    {
        if (!IsValidSelection(state, isCardOrder))
            return;

        if (isCardOrder)
        {
            // Add only selected items
            if (state.SelectedToppings.Count >= MINIMUM_TOPPINGS)
            {
                await AddPizzaWithToppingsToCart(state);
            }
            if (state.SelectedSide != null)
                await CartService.AddSide(CART_KEY, state.SelectedSide);
            if (state.SelectedDrink != null)
                await CartService.AddDrink(CART_KEY, state.SelectedDrink);
        }
        else
        {
            // Swipe: add all three
            await AddPizzaWithToppingsToCart(state);
            await CartService.AddSide(CART_KEY, state.SelectedSide!);
            await CartService.AddDrink(CART_KEY, state.SelectedDrink!);
        }

        ClearSelections(state, Entrees);
    }

    private async Task AddPizzaWithToppingsToCart(SelectionState state)
    {
        if (state.SelectedEntree == null && Entrees.Any())
        {
            state.SelectedEntree = Entrees.First();
        }

        if (state.SelectedEntree == null)
            return;

        await CartService.AddEntree(CART_KEY, state.SelectedEntree);

        foreach (var topping in state.SelectedToppings)
        {
            var toppingOption = AllEntreeOptions.FirstOrDefault(o => o.FoodOptionName == topping);
            if (toppingOption != null)
            {
                var optionType = new FoodOptionTypeDto
                {
                    FoodOptionTypeName = "Pizza Toppings",
                    EntreeId = state.SelectedEntree.Id
                };
                await CartService.AddEntreeOption(CART_KEY, state.SelectedEntree.Id, toppingOption, optionType);
            }
        }
    }

    public override void ClearSelections(SelectionState state, List<EntreeDto> entrees)
    {
        state.SelectedSide = null;
        state.SelectedDrink = null;
        state.SelectedToppings.Clear();

        // Re-select first entree
        if (entrees.Any())
        {
            state.SelectedEntree = entrees.First();
        }
    }

    public override string GetSelectionSummary(SelectionState state)
    {
        if (!IsValidSelection(state, true))
        {
            return "Complete all required fields";
        }

        return $"Personal Pizza with {state.SelectedToppings.Count} topping(s)";
    }

    public override decimal GetExtraToppingCharge(SelectionState state)
    {
        int extraToppings = Math.Max(0, state.SelectedToppings.Count - INCLUDED_TOPPINGS);
        return extraToppings * EXTRA_TOPPING_PRICE;
    }

    public override bool HasExtraToppingCharge(SelectionState state)
    {
        return state.SelectedToppings.Count > INCLUDED_TOPPINGS;
    }
}
