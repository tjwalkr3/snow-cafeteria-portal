using Cafeteria.Customer.Components.Pages.Stations.Configuration;
using Cafeteria.Customer.Components.Pages.Stations.Domain;
using Cafeteria.Customer.Services;
using Cafeteria.Customer.Services.Cart;
using Cafeteria.Customer.Services.Menu;
using Cafeteria.Shared.DTOs;

namespace Cafeteria.Customer.Components.Pages.Stations.Strategies;

public class GrillSelectionStrategy : BaseSelectionStrategy
{
    public override StationType StationType => StationType.Grill;

    public GrillSelectionStrategy(ICartService cartService, IApiMenuService menuService)
        : base(cartService, menuService)
    {
    }

    public override bool IsValidSelection(SelectionState state, bool isCardOrder)
    {
        if (isCardOrder)
        {
            // Card orders: allow any selection
            return state.SelectedEntree != null || state.SelectedSide != null || state.SelectedDrink != null;
        }

        // Swipe orders: require all three
        return state.SelectedEntree != null && state.SelectedSide != null && state.SelectedDrink != null;
    }

    public override async Task AddToCartAsync(SelectionState state, bool isCardOrder)
    {
        if (!IsValidSelection(state, isCardOrder))
            return;

        if (isCardOrder)
        {
            // Add only selected items
            if (state.SelectedEntree != null)
                await CartService.AddEntree(CART_KEY, state.SelectedEntree);
            if (state.SelectedSide != null)
                await CartService.AddSide(CART_KEY, state.SelectedSide);
            if (state.SelectedDrink != null)
                await CartService.AddDrink(CART_KEY, state.SelectedDrink);
        }
        else
        {
            // Swipe: add all three
            await CartService.AddEntree(CART_KEY, state.SelectedEntree!);
            await CartService.AddSide(CART_KEY, state.SelectedSide!);
            await CartService.AddDrink(CART_KEY, state.SelectedDrink!);
        }

        ClearSelections(state, Entrees);
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
