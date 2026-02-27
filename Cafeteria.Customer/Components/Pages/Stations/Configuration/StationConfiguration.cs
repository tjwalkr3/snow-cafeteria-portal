using Cafeteria.Customer.Components.Pages.Stations.Domain;

namespace Cafeteria.Customer.Components.Pages.Stations.Configuration;

public enum OptionSelectionMode
{
    None,           // Grill - no options
    SingleSelect,   // Breakfast - one option per type
    MultiSelect,    // Pizza - multiple toppings
    Mixed           // Deli - combination of single and multi
}

public enum FooterDisplayMode
{
    Standard,       // Shows item count or "Complete all required fields"
    PizzaSummary,   // "Personal Pizza with X topping(s)"
    DeliSummary     // "Custom Sandwich: Bread, Meat, etc."
}

public class StationConfiguration
{
    public StationType StationType { get; init; }
    public string DisplayName { get; init; } = string.Empty;
    public string IconClass { get; init; } = string.Empty;
    public string PageTitle { get; init; } = string.Empty;

    public List<TabDefinition> Tabs { get; init; } = new();
    public string DefaultTab => Tabs.FirstOrDefault(t => t.IsDefault)?.Id ?? Tabs.FirstOrDefault()?.Id ?? "entrees";

    // Entree selection behavior
    public bool ShowEntreeSelection { get; init; } = true;
    public bool AutoSelectFirstEntree { get; init; }
    public bool EntreeSelectionLoadsOptions { get; init; }

    // Option selection behavior
    public OptionSelectionMode OptionSelectionMode { get; init; } = OptionSelectionMode.None;

    // For multi-select stations (Pizza, Deli)
    public int MinimumToppingsRequired { get; init; }
    public int IncludedToppingsCount { get; init; }

    // Footer display
    public FooterDisplayMode FooterDisplayMode { get; init; } = FooterDisplayMode.Standard;

    // Fallback toppings for Pizza (when database returns none)
    public List<string> FallbackToppings { get; init; } = new();

    // Deli-specific: create virtual "Custom Deli Sandwich" entree
    public bool CreateVirtualEntree { get; init; }
    public string VirtualEntreeName { get; init; } = string.Empty;
}
