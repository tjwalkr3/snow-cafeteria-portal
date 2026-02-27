using Cafeteria.Customer.Components.Pages.Stations.Configuration;
using Cafeteria.Customer.Components.Pages.Stations.Domain;
using Cafeteria.Customer.Services;
using Cafeteria.Customer.Services.Cart;
using Cafeteria.Customer.Services.Menu;
using Cafeteria.Shared.DTOs.Menu;

namespace Cafeteria.Customer.Components.Pages.Stations.Strategies;

public abstract class BaseSelectionStrategy : ISelectionStrategy
{
    protected readonly ICartService CartService;
    protected readonly IApiMenuService MenuService;
    protected const string CART_KEY = "order";

    protected int StationId { get; set; }
    protected int LocationId { get; set; }
    protected List<EntreeDto> Entrees { get; set; } = new();
    protected List<FoodOptionTypeWithOptionsDto> OptionTypes { get; set; } = new();
    protected List<FoodOptionDto> AllEntreeOptions { get; set; } = new();
    protected List<string> AvailableToppings { get; set; } = new();

    public abstract StationType StationType { get; }

    protected BaseSelectionStrategy(ICartService cartService, IApiMenuService menuService)
    {
        CartService = cartService;
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

    protected async Task AddEntreeWithOptionsToCart(SelectionState state)
    {
        if (state.SelectedEntree == null) return;

        await CartService.AddEntree(CART_KEY, state.SelectedEntree);

        foreach (var optionType in OptionTypes)
        {
            if (state.SingleSelectOptions.TryGetValue(optionType.OptionType.Id, out var selectedOptionName))
            {
                var option = optionType.Options.FirstOrDefault(o => o.FoodOptionName == selectedOptionName);
                if (option != null)
                {
                    await CartService.AddEntreeOption(CART_KEY, state.SelectedEntree.Id, option, optionType.OptionType);
                }
            }
        }
    }

    protected async Task AddBasicItemsToCart(SelectionState state)
    {
        if (state.SelectedEntree != null)
            await CartService.AddEntree(CART_KEY, state.SelectedEntree);
        if (state.SelectedSide != null)
            await CartService.AddSide(CART_KEY, state.SelectedSide);
        if (state.SelectedDrink != null)
            await CartService.AddDrink(CART_KEY, state.SelectedDrink);
    }
}
