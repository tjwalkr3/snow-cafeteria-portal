using Cafeteria.Customer.Components.Pages.Stations.Configuration;
using Cafeteria.Customer.Services;

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
            StationType.Deli => new DeliSelectionStrategy(_cartService, _menuService),
            _ => throw new ArgumentException($"Unknown station type: {stationType}")
        };
    }
}
