using Microsoft.AspNetCore.Components;

namespace Cafeteria.Customer.Components.Pages.Stations.Footer;

public partial class Footer : ComponentBase
{
    [Parameter, EditorRequired]
    public bool IsCardOrder { get; set; }

    /// <summary>Whether the current tab is the first in the sequence (hides Back button).</summary>
    [Parameter, EditorRequired]
    public bool IsFirstTab { get; set; }

    /// <summary>Whether the current tab is the last in the sequence (switches Next to Add to Order).</summary>
    [Parameter, EditorRequired]
    public bool IsLastTab { get; set; }

    /// <summary>Whether the active tab's completion criteria are met (enables the Next button).</summary>
    [Parameter, EditorRequired]
    public bool CanGoNext { get; set; }

    /// <summary>Whether enough selections have been made to submit the order.</summary>
    [Parameter, EditorRequired]
    public bool CanAddToOrder { get; set; }

    [Parameter, EditorRequired]
    public EventCallback OnAddToOrder { get; set; }

    [Parameter, EditorRequired]
    public EventCallback OnNext { get; set; }

    [Parameter, EditorRequired]
    public EventCallback OnPrevious { get; set; }
}
