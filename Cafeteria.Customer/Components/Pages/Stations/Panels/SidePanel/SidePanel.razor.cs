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

    private Task HandleSideTap(SideWithOptionsDto sideWithOptions)
    {
        if (sideWithOptions.OptionTypes.Any())
            return OnSideWithOptionsSelected.InvokeAsync(sideWithOptions);
        return OnSideSelected.InvokeAsync(sideWithOptions.Side);
    }
}
