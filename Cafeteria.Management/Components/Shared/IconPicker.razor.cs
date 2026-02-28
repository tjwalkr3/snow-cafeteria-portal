using Microsoft.AspNetCore.Components;

namespace Cafeteria.Management.Components.Shared;

public partial class IconPicker : ComponentBase
{
    [Parameter]
    public string? Value { get; set; }

    [Parameter]
    public EventCallback<string?> ValueChanged { get; set; }

    private string searchQuery = string.Empty;

    private record IconEntry(string ClassName, string Label);

    private static readonly List<IconEntry> AllIcons =
    [
        // Location & Buildings
        new("bi-geo-alt-fill",      "Location Pin"),
        new("bi-geo-alt",           "Location Pin (outline)"),
        new("bi-pin-map-fill",      "Pin Map"),
        new("bi-pin-map",           "Pin Map (outline)"),
        new("bi-house-door-fill",   "House Door"),
        new("bi-house-fill",        "House"),
        new("bi-building-fill",     "Building"),
        new("bi-building",          "Building (outline)"),
        new("bi-shop",              "Shop"),
        new("bi-shop-window",       "Shop Window"),
        // Food & Drink
        new("bi-fire",              "Fire / Grill"),
        new("bi-cup-hot-fill",      "Hot Drink"),
        new("bi-cup-hot",           "Hot Drink (outline)"),
        new("bi-cup-straw",         "Cup with Straw"),
        new("bi-cup-fill",          "Cup"),
        new("bi-cup",               "Cup (outline)"),
        new("bi-egg-fried",         "Fried Egg / Deli"),
        new("bi-egg-fill",          "Egg"),
        new("bi-pie-chart-fill",    "Pie / Pizza"),
        new("bi-pie-chart",         "Pie Chart (outline)"),
        new("bi-tornado",           "Tornado / Wraps"),
        new("bi-basket-fill",       "Basket"),
        new("bi-basket2-fill",      "Basket 2"),
        new("bi-cart-fill",         "Cart"),
        new("bi-bag-fill",          "Bag"),
        new("bi-bag-heart-fill",    "Bag Heart"),
        // People & Social
        new("bi-people-fill",       "People"),
        new("bi-person-fill",       "Person"),
        new("bi-person-circle",     "Person Circle"),
        // Misc
        new("bi-book-fill",         "Book"),
        new("bi-book",              "Book (outline)"),
        new("bi-star-fill",         "Star"),
        new("bi-heart-fill",        "Heart"),
        new("bi-lightning-fill",    "Lightning"),
        new("bi-sun-fill",          "Sun"),
        new("bi-moon-fill",         "Moon"),
        new("bi-circle-fill",       "Circle"),
        new("bi-diamond-fill",      "Diamond"),
        new("bi-trophy-fill",       "Trophy"),
        new("bi-award-fill",        "Award"),
        new("bi-flower1",           "Flower"),
        new("bi-flower2",           "Flower 2"),
        new("bi-tree-fill",         "Tree"),
        new("bi-bell-fill",         "Bell"),
        new("bi-clock-fill",        "Clock"),
        new("bi-calendar-fill",     "Calendar"),
        new("bi-tag-fill",          "Tag"),
        new("bi-grid-fill",         "Grid"),
        new("bi-list",              "List"),
        new("bi-info-circle-fill",  "Info"),
    ];

    private IEnumerable<IconEntry> FilteredIcons =>
        string.IsNullOrWhiteSpace(searchQuery)
            ? AllIcons
            : AllIcons.Where(i =>
                i.Label.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ||
                i.ClassName.Contains(searchQuery, StringComparison.OrdinalIgnoreCase));

    private async Task SelectIcon(string className)
    {
        Value = className;
        await ValueChanged.InvokeAsync(className);
    }

    private async Task ClearIcon()
    {
        Value = null;
        await ValueChanged.InvokeAsync(null);
    }
}
