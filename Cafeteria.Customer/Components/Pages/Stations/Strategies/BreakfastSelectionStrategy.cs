using Cafeteria.Customer.Components.Pages.Stations.Domain;
using Cafeteria.Customer.Services.Menu;
using Cafeteria.Shared.DTOs.Menu;

namespace Cafeteria.Customer.Components.Pages.Stations.Strategies;

public class BreakfastSelectionStrategy : BaseSelectionStrategy
{
    public BreakfastSelectionStrategy(CartSubmitter cartSubmitter, IApiMenuService menuService)
        : base(cartSubmitter, menuService)
    {
    }

    public override bool IsValidSelection(SelectionState state, bool isCardOrder) =>
        SelectionValidator.IsValid(
            state, OptionTypes, isCardOrder,
            requiresOptionsComplete: true);

    public override async Task OnEntreeSelectedAsync(EntreeDto entree, SelectionState state)
    {
        state.SelectedEntree = entree;
        state.SingleSelectOptions.Clear();

        AllEntreeOptions = await MenuService.GetOptionsByEntree(entree.Id);
        OptionTypes = await MenuService.GetOptionTypesWithOptionsByEntree(entree.Id);
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
        OptionTypes.Clear();
        AllEntreeOptions.Clear();
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
