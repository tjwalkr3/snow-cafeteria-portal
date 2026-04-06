using Cafeteria.Customer.Services.Cart;
using Cafeteria.Shared.DTOs.Menu;

namespace Cafeteria.Customer.Components.Pages.Stations.Domain;

public class CartSubmitter
{
    private readonly ICartService _cartService;

    public CartSubmitter(ICartService cartService)
    {
        _cartService = cartService;
    }

    public async Task SubmitAsync(
        string cartKey,
        SelectionState state,
        List<FoodOptionTypeWithOptionsDto> optionTypes,
        List<FoodOptionTypeWithOptionsDto>? sideOptionTypes = null)
    {
        if (state.SelectedEntree != null)
            await AddEntreeAsync(cartKey, state, optionTypes);

        if (state.SelectedSide != null)
            await AddSideAsync(cartKey, state, sideOptionTypes);

        if (state.SelectedDrink != null)
            await _cartService.AddDrink(cartKey, state.SelectedDrink);
    }

    private async Task AddSideAsync(
        string cartKey,
        SelectionState state,
        List<FoodOptionTypeWithOptionsDto>? sideOptionTypes)
    {
        await _cartService.AddSide(cartKey, state.SelectedSide!);

        if (sideOptionTypes == null || state.SideOptions.Count == 0)
            return;

        foreach (var optionType in sideOptionTypes)
        {
            if (!state.SideOptions.TryGetValue(optionType.OptionType.Id, out var selections))
                continue;

            foreach (var name in selections)
            {
                var option = optionType.Options.FirstOrDefault(o => o.FoodOptionName == name);
                if (option != null)
                    await _cartService.AddSideOption(cartKey, state.SelectedSide!.Id, option, optionType.OptionType);
            }
        }
    }

    private async Task AddEntreeAsync(
        string cartKey,
        SelectionState state,
        List<FoodOptionTypeWithOptionsDto> optionTypes)
    {
        await _cartService.AddEntree(cartKey, state.SelectedEntree!);

        foreach (var optionType in optionTypes)
        {
            if (optionType.OptionType.MaxAmount > 1)
            {
                if (!state.MultiSelectOptions.TryGetValue(optionType.OptionType.Id, out var selections))
                    continue;

                foreach (var name in selections)
                {
                    var option = optionType.Options.FirstOrDefault(o => o.FoodOptionName == name);
                    if (option != null)
                        await _cartService.AddEntreeOption(cartKey, state.SelectedEntree!.Id, option, optionType.OptionType);
                }
            }
            else
            {
                if (!state.SingleSelectOptions.TryGetValue(optionType.OptionType.Id, out var name)
                    || string.IsNullOrEmpty(name))
                    continue;

                var option = optionType.Options.FirstOrDefault(o => o.FoodOptionName == name);
                if (option != null)
                    await _cartService.AddEntreeOption(cartKey, state.SelectedEntree!.Id, option, optionType.OptionType);
            }
        }
    }
}
