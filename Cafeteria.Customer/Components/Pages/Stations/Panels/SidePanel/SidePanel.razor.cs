using Cafeteria.Shared.DTOs.Menu;
using Microsoft.AspNetCore.Components;

namespace Cafeteria.Customer.Components.Pages.Stations.Panels.SidePanel;

public partial class SidePanel : ComponentBase
{
    [Parameter, EditorRequired]
    public List<SideDto> Sides { get; set; } = new();

    [Parameter, EditorRequired]
    public SideDto? SelectedSide { get; set; }

    [Parameter, EditorRequired]
    public bool IsCardOrder { get; set; }

    [Parameter, EditorRequired]
    public EventCallback<SideDto> OnSideSelected { get; set; }
}
