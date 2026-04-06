using Cafeteria.Shared.DTOs.Menu;
using Microsoft.AspNetCore.Components;

namespace Cafeteria.Customer.Components.Pages.Stations.Panels.SidePanel;

public partial class SidePanel : ComponentBase
{
    [Parameter, EditorRequired]
    public List<SideWithOptionsDto> Sides { get; set; } = new();

    [Parameter, EditorRequired]
    public SideDto? SelectedSide { get; set; }

    [Parameter, EditorRequired]
    public bool IsCardOrder { get; set; }

    [Parameter, EditorRequired]
    public EventCallback<SideDto> OnSideSelected { get; set; }

    [Parameter]
    public EventCallback<SideWithOptionsDto> OnSideWithOptionsSelected { get; set; }

    [Parameter]
    public int Quantity { get; set; }

    [Parameter]
    public EventCallback<int> OnQuantityChanged { get; set; }

    [Parameter]
    public IReadOnlyDictionary<int, int>? CardQuantities { get; set; }

    [Parameter]
    public EventCallback<(int SideId, int NewQty)> OnCardQtyChanged { get; set; }

    /// <summary>Card order options for each side (sideId -> optionTypeId -> selected option names).</summary>
    [Parameter]
    public IReadOnlyDictionary<int, Dictionary<int, HashSet<string>>>? CardSideOptions { get; set; }

    private Task HandleSideTap(SideWithOptionsDto sideWithOptions)
    {
        if (sideWithOptions.OptionTypes.Any())
            return OnSideWithOptionsSelected.InvokeAsync(sideWithOptions);
        return OnSideSelected.InvokeAsync(sideWithOptions.Side);
    }

    /// <summary>Get the flattened list of selected option names for a specific side in a card order.</summary>
    private List<string> GetCardSideOptions(int sideId)
    {
        if (CardSideOptions?.TryGetValue(sideId, out var optionsByType) != true || optionsByType == null)
            return [];

        return optionsByType
            .Values
            .SelectMany(options => options)
            .ToList();
    }
}
