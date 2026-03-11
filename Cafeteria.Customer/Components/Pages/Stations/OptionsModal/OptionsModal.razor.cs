using Cafeteria.Customer.Components.Pages.Stations.Domain;
using Cafeteria.Shared.DTOs.Menu;
using Microsoft.AspNetCore.Components;

namespace Cafeteria.Customer.Components.Pages.Stations.OptionsModal;

public partial class OptionsModal : ComponentBase
{
    [Parameter, EditorRequired]
    public List<FoodOptionTypeWithOptionsDto> OptionTypes { get; set; } = new();

    [Parameter, EditorRequired]
    public bool IsCardOrder { get; set; }

    [Parameter, EditorRequired]
    public FoodOptionStagingStore StagingStore { get; set; } = default!;

    [Parameter, EditorRequired]
    public EventCallback OnConfirm { get; set; }

    [Parameter, EditorRequired]
    public EventCallback OnCancel { get; set; }

    private bool IsAllComplete =>
        OptionTypes.All(ot =>
            IsOptionTypeComplete(ot, (StagingStore.StagedSelections.GetValueOrDefault(ot.OptionType.Id) ?? new HashSet<string>()).Count));

    private static bool IsOptionTypeComplete(FoodOptionTypeWithOptionsDto optionTypeWithOptions, int stagedCount)
    {
        var required = optionTypeWithOptions.OptionType.RequiredAmount;
        return required == 0 || stagedCount >= required;
    }

    private static string GetHintText(FoodOptionTypeDto optType, bool isMulti)
    {
        if (!isMulti)
            return "— Select 1";
        if (optType.RequiredAmount == 0)
            return $"— Optional, up to {optType.MaxAmount}";
        if (optType.MaxAmount > optType.RequiredAmount)
            return $"— Select at least {optType.RequiredAmount}, up to {optType.MaxAmount}";
        return $"— Select {optType.RequiredAmount}";
    }

    private void Toggle(int optionTypeId, string name)
    {
        var optionType = OptionTypes.FirstOrDefault(o => o.OptionType.Id == optionTypeId);
        if (optionType != null)
            StagingStore.Toggle(optionTypeId, name, optionType, IsCardOrder);
    }

    private void Set(int optionTypeId, string name) =>
        StagingStore.Set(optionTypeId, name);
}
