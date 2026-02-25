using Cafeteria.Customer.Components.Pages.Stations.Configuration;
using Cafeteria.Customer.Services;
using Cafeteria.Customer.Services.Cart;
using Cafeteria.Customer.Services.Menu;

namespace Cafeteria.Customer.Components.Pages.Stations.Strategies;

public class SelectionStrategyFactory : ISelectionStrategyFactory
{
    private readonly ICartService _cartService;
    private readonly IApiMenuService _menuService;

    public SelectionStrategyFactory(ICartService cartService, IApiMenuService menuService)
    {
        _cartService = cartService;
        _menuService = menuService;
    }

    public ISelectionStrategy CreateStrategy(StationType stationType)
    {
        return stationType switch
        {
            StationType.Grill => new GrillSelectionStrategy(_cartService, _menuService),
            StationType.Breakfast => new BreakfastSelectionStrategy(_cartService, _menuService),
            StationType.Pizza => new PizzaSelectionStrategy(_cartService, _menuService),
            StationType.Deli => new OptionBuilderSelectionStrategy(
                _cartService, _menuService, StationType.Deli,
                e => e.EntreeName.Contains("Sandwich") || e.EntreeName.Contains("Deli"),
                "Custom Deli Sandwich"),
            StationType.Wraps => new OptionBuilderSelectionStrategy(
                _cartService, _menuService, StationType.Wraps,
                e => e.EntreeName.Contains("Wrap", StringComparison.OrdinalIgnoreCase),
                "Custom Wrap"),
            _ => throw new ArgumentException($"Unknown station type: {stationType}")
        };
    }
}
