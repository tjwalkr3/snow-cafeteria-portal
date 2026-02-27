using Cafeteria.Customer.Components.Pages.Stations.Configuration;
using Cafeteria.Customer.Components.Pages.Stations.Domain;
using Cafeteria.Customer.Services.Menu;
using Cafeteria.Shared.DTOs;

namespace Cafeteria.Customer.Components.Pages.Stations.Strategies;

public class GrillSelectionStrategy : BaseSelectionStrategy
{
    public override StationType StationType => StationType.Grill;

    public GrillSelectionStrategy(CartSubmitter cartSubmitter, IApiMenuService menuService)
        : base(cartSubmitter, menuService)
    {
    }

    public override bool IsValidSelection(SelectionState state, bool isCardOrder) =>
        SelectionValidator.IsValid(
            state, OptionTypes, isCardOrder,
            requiresOptionsComplete: false);

    public override async Task AddToCartAsync(SelectionState state, bool isCardOrder)
    {
        if (!IsValidSelection(state, isCardOrder))
            return;

        await CartSubmitter.SubmitAsync(state, OptionTypes, AllEntreeOptions);
        ClearSelections(state, Entrees);
    }

    public override string GetSelectionSummary(SelectionState state)
    {
        if (!IsValidSelection(state, true))
            return "Complete all required fields";

        var items = new List<string>();
        if (state.SelectedEntree != null) items.Add(state.SelectedEntree.EntreeName);
        if (state.SelectedSide != null) items.Add(state.SelectedSide.SideName);
        if (state.SelectedDrink != null) items.Add(state.SelectedDrink.DrinkName);

        return string.Join(", ", items);
    }
}
