using Cafeteria.Customer.Components.Pages.Stations.Domain;
using Cafeteria.Customer.Services.Menu;
using Cafeteria.Shared.DTOs.Menu;

namespace Cafeteria.Customer.Components.Pages.Stations.Strategies;

public abstract class BaseSelectionStrategy : ISelectionStrategy
{
    protected readonly CartSubmitter CartSubmitter;
    protected readonly IApiMenuService MenuService;

    protected int StationId { get; set; }
    protected int LocationId { get; set; }
    protected List<EntreeDto> Entrees { get; set; } = new();
    protected List<FoodOptionTypeWithOptionsDto> OptionTypes { get; set; } = new();
    protected List<FoodOptionDto> AllEntreeOptions { get; set; } = new();
    protected List<string> AvailableToppings { get; set; } = new();

    protected BaseSelectionStrategy(CartSubmitter cartSubmitter, IApiMenuService menuService)
    {
        CartSubmitter = cartSubmitter;
        MenuService = menuService;
    }

    public void SetStationInfo(int stationId, int locationId)
    {
        StationId = stationId;
        LocationId = locationId;
    }

    public abstract bool IsValidSelection(SelectionState state, bool isCardOrder);

    public virtual int GetSelectionCount(SelectionState state)
    {
        int count = 0;
        if (state.SelectedEntree != null) count++;
        if (state.SelectedSide != null) count++;
        if (state.SelectedDrink != null) count++;
        return count;
    }

    public virtual string GetSelectionSummary(SelectionState state)
    {
        return $"{GetSelectionCount(state)} item(s) selected";
    }

    public abstract Task AddToCartAsync(SelectionState state, bool isCardOrder);

    public virtual Task OnDataLoadedAsync(
        List<EntreeDto> entrees,
        List<SideDto> sides,
        List<DrinkDto> drinks,
        SelectionState state)
    {
        Entrees = entrees;
        return Task.CompletedTask;
    }

    public virtual void ClearSelections(SelectionState state, List<EntreeDto> entrees)
    {
        state.Clear();
    }

    public virtual Task OnEntreeSelectedAsync(EntreeDto entree, SelectionState state)
    {
        state.SelectedEntree = entree;
        return Task.CompletedTask;
    }

    public virtual void SetOptionForType(int optionTypeId, string optionName, SelectionState state)
    {
        state.SingleSelectOptions[optionTypeId] = optionName;
    }

    public virtual void ToggleOptionForType(int optionTypeId, string optionName, SelectionState state)
    {
        if (!state.MultiSelectOptions.ContainsKey(optionTypeId))
        {
            state.MultiSelectOptions[optionTypeId] = new List<string>();
        }

        var selectedOptions = state.MultiSelectOptions[optionTypeId];
        if (selectedOptions.Contains(optionName))
        {
            selectedOptions.Remove(optionName);
        }
        else
        {
            selectedOptions.Add(optionName);
        }
    }

    public virtual void ToggleTopping(string topping, SelectionState state)
    {
        if (state.SelectedToppings.Contains(topping))
        {
            state.SelectedToppings.Remove(topping);
        }
        else
        {
            state.SelectedToppings.Add(topping);
        }
    }

    public virtual List<FoodOptionTypeWithOptionsDto> GetOptionTypes() => OptionTypes;

    public virtual List<string> GetAvailableToppings() => AvailableToppings;

    public virtual decimal GetExtraToppingCharge(SelectionState state) => 0m;

    public virtual bool HasExtraToppingCharge(SelectionState state) => false;
}
