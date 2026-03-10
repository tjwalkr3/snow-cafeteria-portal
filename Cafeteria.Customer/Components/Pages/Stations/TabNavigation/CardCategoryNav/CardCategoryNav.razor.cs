using Cafeteria.Customer.Components.Pages.Stations.Domain;
using Microsoft.AspNetCore.Components;

namespace Cafeteria.Customer.Components.Pages.Stations.TabNavigation.CardCategoryNav;

public partial class CardCategoryNav : ComponentBase
{
    [Parameter, EditorRequired]
    public List<TabDefinition> Tabs { get; set; } = new();

    [Parameter, EditorRequired]
    public string ActiveTab { get; set; } = string.Empty;

    [Parameter, EditorRequired]
    public Func<string, bool> HasSelectionForTab { get; set; } = _ => false;

    [Parameter, EditorRequired]
    public Func<string, string> GetSelectionText { get; set; } = _ => string.Empty;

    [Parameter, EditorRequired]
    public Func<string, string> GetTabIcon { get; set; } = _ => "bi-circle";

    [Parameter]
    public EventCallback<string> OnTabSelected { get; set; }
}
