using Cafeteria.Customer.Services;
using Cafeteria.Customer.Services.Menu;
using Cafeteria.Shared.DTOs.Menu;
using Cafeteria.Shared.DTOs.Order;
using Cafeteria.Shared.Utilities;

namespace Cafeteria.Customer.Components.Pages.PlaceOrder;

public class PlaceOrderVM : IPlaceOrderVM
{
    private readonly IApiMenuService _menuService;
    private bool locationFetchFailed = false;
    private List<LocationDto> _locations = new();

    public PlaceOrderVM(IApiMenuService menuService)
    {
        _menuService = menuService;
    }

    public decimal CalculateTotalPrice(BrowserOrder order) =>
        OrderCalculations.CalculateTotalPrice(order);

    public async Task InitializeLocations()
    {
        try
        {
            _locations = await _menuService.GetAllLocations();
        }
        catch
        {
            locationFetchFailed = true;
        }
    }

    public LocationDto? GetLocationById(int locationId)
    {
        return _locations.FirstOrDefault(l => l.Id == locationId);
    }

    public bool ErrorOccurred()
    {
        return locationFetchFailed;
    }

    public List<SwipeGroup> GroupItemsIntoSwipes(BrowserOrder order)
    {
        if (order == null || order.IsCardOrder)
            return new List<SwipeGroup>();

        var groupMap = new Dictionary<string, SwipeGroup>();
        bool hasSides = order.Sides.Any();

        int swipeCount = hasSides
            ? Math.Min(order.Entrees.Count, Math.Min(order.Sides.Count, order.Drinks.Count))
            : Math.Min(order.Entrees.Count, order.Drinks.Count);

        for (int i = 0; i < swipeCount; i++)
        {
            var swipe = new SwipeGroup
            {
                Entree = order.Entrees[i],
                Side = hasSides ? order.Sides[i] : null,
                Drink = order.Drinks[i],
                Quantity = 1
            };

            string key = swipe.GroupKey;
            if (groupMap.ContainsKey(key))
                groupMap[key].Quantity++;
            else
                groupMap[key] = swipe;
        }

        return groupMap.Values.ToList();
    }

    // Card order item grouping methods
    public List<EntreeGroup> GroupEntrees(BrowserOrder order)
    {
        if (order == null || !order.IsCardOrder)
            return new List<EntreeGroup>();

        var groupMap = new Dictionary<string, EntreeGroup>();

        foreach (var entreeItem in order.Entrees)
        {
            var group = new EntreeGroup
            {
                Entree = entreeItem,
                Quantity = 1
            };

            string key = group.GroupKey;
            if (groupMap.ContainsKey(key))
                groupMap[key].Quantity++;
            else
                groupMap[key] = group;
        }

        return groupMap.Values.ToList();
    }

    public List<SideGroup> GroupSides(BrowserOrder order)
    {
        if (order == null || !order.IsCardOrder)
            return new List<SideGroup>();

        var groupMap = new Dictionary<string, SideGroup>();

        foreach (var sideItem in order.Sides)
        {
            var group = new SideGroup
            {
                Side = sideItem,
                Quantity = 1
            };

            string key = group.GroupKey;
            if (groupMap.ContainsKey(key))
                groupMap[key].Quantity++;
            else
                groupMap[key] = group;
        }

        return groupMap.Values.ToList();
    }

    public List<DrinkGroup> GroupDrinks(BrowserOrder order)
    {
        if (order == null || !order.IsCardOrder)
            return new List<DrinkGroup>();

        var groupMap = new Dictionary<string, DrinkGroup>();

        foreach (var drink in order.Drinks)
        {
            var group = new DrinkGroup
            {
                Drink = drink,
                Quantity = 1
            };

            string key = group.GroupKey;
            if (groupMap.ContainsKey(key))
                groupMap[key].Quantity++;
            else
                groupMap[key] = group;
        }

        return groupMap.Values.ToList();
    }

    public int CalculateTotalSwipe(BrowserOrder order)
    {
        if (order == null || order.IsCardOrder)
            return 0;

        return OrderCalculations.CalculateTotalSwipe(order);
    }

    public decimal CalculateTax(BrowserOrder order) =>
        OrderCalculations.CalculateTax(order);
}

public class SwipeGroup
{
    public OrderEntreeItem Entree { get; set; } = new();
    public OrderSideItem? Side { get; set; }
    public DrinkDto Drink { get; set; } = new();
    public int Quantity { get; set; } = 1;

    public string GroupKey =>
        $"{Entree?.Entree?.Id ?? 0}-{GetOptionsHash(Entree?.SelectedOptions)}-" +
        $"{Side?.Side?.Id ?? 0}-{GetOptionsHash(Side?.SelectedOptions)}-{Drink?.Id ?? 0}";

    private string GetOptionsHash(List<SelectedFoodOption>? options)
    {
        if (options == null || options.Count == 0)
            return "none";
        return string.Join(",", options.OrderBy(o => o.Option.Id).Select(o => o.Option.Id));
    }
}

public class EntreeGroup
{
    public OrderEntreeItem Entree { get; set; } = new();
    public int Quantity { get; set; } = 1;

    public string GroupKey =>
        $"{Entree?.Entree?.Id ?? 0}-{GetOptionsHash(Entree?.SelectedOptions)}";

    private string GetOptionsHash(List<SelectedFoodOption>? options)
    {
        if (options == null || options.Count == 0)
            return "none";
        return string.Join(",", options.OrderBy(o => o.Option.Id).Select(o => o.Option.Id));
    }
}

public class SideGroup
{
    public OrderSideItem Side { get; set; } = new();
    public int Quantity { get; set; } = 1;

    public string GroupKey =>
        $"{Side?.Side?.Id ?? 0}-{GetOptionsHash(Side?.SelectedOptions)}";

    private string GetOptionsHash(List<SelectedFoodOption>? options)
    {
        if (options == null || options.Count == 0)
            return "none";
        return string.Join(",", options.OrderBy(o => o.Option.Id).Select(o => o.Option.Id));
    }
}

public class DrinkGroup
{
    public DrinkDto Drink { get; set; } = new();
    public int Quantity { get; set; } = 1;

    public string GroupKey => $"{Drink?.Id ?? 0}";
}