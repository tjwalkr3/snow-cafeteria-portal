using Cafeteria.Customer.Components.Pages.Stations.Configuration;
using Cafeteria.Customer.Components.Pages.Stations.Domain;
using Cafeteria.Customer.Services.Menu;

namespace Cafeteria.Customer.Components.Pages.Stations.Strategies;

public class SelectionStrategyFactory : ISelectionStrategyFactory
{
    private readonly CartSubmitter _cartSubmitter;
    private readonly IApiMenuService _menuService;

    public SelectionStrategyFactory(CartSubmitter cartSubmitter, IApiMenuService menuService)
    {
        _cartSubmitter = cartSubmitter;
        _menuService = menuService;
    }

    public ISelectionStrategy CreateStrategy(StationType stationType)
    {
        return stationType switch
        {
            StationType.Grill => new GrillSelectionStrategy(_cartSubmitter, _menuService),
            StationType.Breakfast => new BreakfastSelectionStrategy(_cartSubmitter, _menuService),
            StationType.Pizza => new PizzaSelectionStrategy(_cartSubmitter, _menuService),
            StationType.Deli => new OptionBuilderSelectionStrategy(
                _cartSubmitter, _menuService, StationType.Deli,
                e => e.EntreeName.Contains("Sandwich") || e.EntreeName.Contains("Deli"),
                "Custom Deli Sandwich"),
            StationType.Wraps => new OptionBuilderSelectionStrategy(
                _cartSubmitter, _menuService, StationType.Wraps,
                e => e.EntreeName.Contains("Wrap", StringComparison.OrdinalIgnoreCase),
                "Custom Wrap"),
            _ => throw new ArgumentException($"Unknown station type: {stationType}")
        };
    }
}
