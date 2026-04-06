using Cafeteria.Shared.DTOs.Menu;
using Microsoft.AspNetCore.Components;

namespace Cafeteria.Customer.Components.Pages.Stations.Panels.EntreePanel;

public partial class EntreePanel : ComponentBase
{
    [Parameter, EditorRequired]
    public List<EntreeDto> Entrees { get; set; } = new();

    [Parameter, EditorRequired]
    public EntreeDto? SelectedEntree { get; set; }

    [Parameter, EditorRequired]
    public bool IsCardOrder { get; set; }

    [Parameter, EditorRequired]
    public EventCallback<EntreeDto> OnEntreeSelected { get; set; }

    [Parameter]
    public int Quantity { get; set; }

    [Parameter]
    public EventCallback<int> OnQuantityChanged { get; set; }

    [Parameter]
    public IReadOnlyDictionary<int, int>? CardQuantities { get; set; }

    [Parameter]
    public EventCallback<(int EntreeId, int NewQty)> OnCardQtyChanged { get; set; }

    /// <summary>Chips displayed under the entree name when it is selected.</summary>
    [Parameter]
    public IReadOnlyList<string> SelectedBadges { get; set; } = Array.Empty<string>();

    /// <summary>Card order options for each entree (entreeId -> optionTypeId -> selected option names).</summary>
    [Parameter]
    public IReadOnlyDictionary<int, Dictionary<int, HashSet<string>>>? CardEntreeOptions { get; set; }

    private bool IsComplete(EntreeDto entree) => SelectedEntree?.Id == entree.Id;

    /// <summary>Get the flattened list of selected option names for a specific entree in a card order.</summary>
    private List<string> GetCardEntreeOptions(int entreeId)
    {
        if (CardEntreeOptions?.TryGetValue(entreeId, out var optionsByType) != true || optionsByType == null)
            return [];

        return optionsByType
            .Values
            .SelectMany(options => options)
            .ToList();
    }
}
