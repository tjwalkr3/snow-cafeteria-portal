using Cafeteria.Customer.Components.Pages.Stations.Models;

namespace Cafeteria.Customer.Components.Pages.Stations.Configuration;

public class StationConfigurationProvider : IStationConfigurationProvider
{
    private readonly Dictionary<StationType, StationConfiguration> _configurations;

    public StationConfigurationProvider()
    {
        _configurations = new Dictionary<StationType, StationConfiguration>
        {
            [StationType.Grill] = CreateGrillConfiguration(),
            [StationType.Breakfast] = CreateBreakfastConfiguration(),
            [StationType.Pizza] = CreatePizzaConfiguration(),
            [StationType.Deli] = CreateDeliConfiguration(),
            [StationType.Wraps] = CreateWrapsConfiguration()
        };
    }

    public StationConfiguration GetConfiguration(StationType stationType)
    {
        return _configurations.TryGetValue(stationType, out var config)
            ? config
            : throw new ArgumentException($"Unknown station type: {stationType}");
    }

    public StationType ParseStationType(string stationName)
    {
        if (TryParseStationType(stationName, out var stationType))
        {
            return stationType;
        }
        throw new ArgumentException($"Unknown station name: {stationName}");
    }

    public bool TryParseStationType(string stationName, out StationType stationType)
    {
        stationType = stationName?.ToLowerInvariant() switch
        {
            "grill" => StationType.Grill,
            "breakfast" => StationType.Breakfast,
            "pizza" => StationType.Pizza,
            "deli" => StationType.Deli,
            "wraps" => StationType.Wraps,
            _ => default
        };

        return stationName?.ToLowerInvariant() is "grill" or "breakfast" or "pizza" or "deli" or "wraps";
    }

    private static StationConfiguration CreateGrillConfiguration()
    {
        return new StationConfiguration
        {
            StationType = StationType.Grill,
            DisplayName = "Grill",
            IconClass = "bi-fire",
            PageTitle = "Grill Station",
            Tabs = new List<TabDefinition>
            {
                new("entrees", "Entrees", isDefault: true),
                new("sides", "Sides"),
                new("drinks", "Drinks")
            },
            ShowEntreeSelection = true,
            AutoSelectFirstEntree = false,
            EntreeSelectionLoadsOptions = false,
            OptionSelectionMode = OptionSelectionMode.None,
            FooterDisplayMode = FooterDisplayMode.Standard
        };
    }

    private static StationConfiguration CreateBreakfastConfiguration()
    {
        return new StationConfiguration
        {
            StationType = StationType.Breakfast,
            DisplayName = "Breakfast",
            IconClass = "bi-cup-hot",
            PageTitle = "Breakfast Station",
            Tabs = new List<TabDefinition>
            {
                new("entrees", "Entrees", isDefault: true),
                new("sides", "Sides"),
                new("drinks", "Drinks")
            },
            ShowEntreeSelection = true,
            AutoSelectFirstEntree = false,
            EntreeSelectionLoadsOptions = true,
            OptionSelectionMode = OptionSelectionMode.SingleSelect,
            FooterDisplayMode = FooterDisplayMode.Standard
        };
    }

    private static StationConfiguration CreatePizzaConfiguration()
    {
        return new StationConfiguration
        {
            StationType = StationType.Pizza,
            DisplayName = "Pizza",
            IconClass = "bi-circle-fill",
            PageTitle = "Pizza Station",
            Tabs = new List<TabDefinition>
            {
                new("toppings", "Pizza", isDefault: true),
                new("sides", "Sides"),
                new("drinks", "Drinks")
            },
            ShowEntreeSelection = true,
            AutoSelectFirstEntree = true,
            EntreeSelectionLoadsOptions = false,
            OptionSelectionMode = OptionSelectionMode.MultiSelect,
            MinimumToppingsRequired = 2,
            ExtraToppingPrice = 0.50m,
            IncludedToppingsCount = 2,
            FooterDisplayMode = FooterDisplayMode.PizzaSummary,
            FallbackToppings = new List<string>
            {
                "Extra Cheese", "Pepperoni", "Sausage", "Bacon", "Chicken", "Ham",
                "Olives", "Mushrooms", "Onions", "Pineapple", "Bell Peppers", "Banana Peppers"
            }
        };
    }

    private static StationConfiguration CreateDeliConfiguration()
    {
        return new StationConfiguration
        {
            StationType = StationType.Deli,
            DisplayName = "Deli",
            IconClass = "bi-egg-fried",
            PageTitle = "Deli Station",
            Tabs = new List<TabDefinition>
            {
                new("sandwich", "Entree", isDefault: true),
                new("sides", "Sides"),
                new("drinks", "Drinks")
            },
            ShowEntreeSelection = false,
            AutoSelectFirstEntree = false,
            EntreeSelectionLoadsOptions = false,
            OptionSelectionMode = OptionSelectionMode.Mixed,
            FooterDisplayMode = FooterDisplayMode.DeliSummary,
            CreateVirtualEntree = true,
            VirtualEntreeName = "Custom Deli Sandwich"
        };
    }

    private static StationConfiguration CreateWrapsConfiguration()
    {
        return new StationConfiguration
        {
            StationType = StationType.Wraps,
            DisplayName = "Wraps",
            IconClass = "bi-tornado",
            PageTitle = "Wraps Station",
            Tabs = new List<TabDefinition>
            {
                new("wrap", "Wrap", isDefault: true),
                new("sides", "Sides"),
                new("drinks", "Drinks")
            },
            ShowEntreeSelection = false,
            AutoSelectFirstEntree = false,
            EntreeSelectionLoadsOptions = false,
            OptionSelectionMode = OptionSelectionMode.Mixed,
            FooterDisplayMode = FooterDisplayMode.DeliSummary,
            CreateVirtualEntree = true,
            VirtualEntreeName = "Custom Wrap"
        };
    }
}
