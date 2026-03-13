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

    /// <summary>Chips displayed under the entree name when it is selected.</summary>
    [Parameter]
    public IReadOnlyList<string> SelectedBadges { get; set; } = Array.Empty<string>();

    /// <summary>
    /// When greater than zero, completion is determined by SelectionCount >= RequiredCount
    /// rather than simply by whether the entree is selected.
    /// </summary>
    [Parameter]
    public int SelectionCount { get; set; }

    [Parameter]
    public int RequiredCount { get; set; }

    private bool IsComplete(EntreeDto entree)
    {
        if (SelectedEntree?.Id != entree.Id) return false;
        return RequiredCount == 0 || SelectionCount >= RequiredCount;
    }

    private bool IsPending(EntreeDto entree) =>
        SelectedEntree?.Id == entree.Id && RequiredCount > 0 && SelectionCount < RequiredCount;
}
