using Cafeteria.Customer.Components.Pages.Stations.Domain;
using Microsoft.AspNetCore.Components;

namespace Cafeteria.Customer.Components.Pages.Stations.TabNavigation.SwipeStepper;

public partial class SwipeStepper : ComponentBase
{
    [Parameter, EditorRequired]
    public List<TabDefinition> Tabs { get; set; } = new();

    [Parameter, EditorRequired]
    public string ActiveTab { get; set; } = string.Empty;

    [Parameter, EditorRequired]
    public Func<string, bool> IsTabCompleted { get; set; } = _ => false;

    [Parameter]
    public EventCallback<string> OnTabSelected { get; set; }
}
