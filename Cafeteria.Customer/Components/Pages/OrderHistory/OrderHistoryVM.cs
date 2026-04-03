using Cafeteria.Customer.Services.Order;
using Cafeteria.Customer.Services.Menu;
using Cafeteria.Shared.DTOs.Order;

namespace Cafeteria.Customer.Components.Pages.OrderHistory;

public class OrderHistoryVM : IOrderHistoryVM
{
    private readonly IApiOrderService _orderService;
    private readonly IApiMenuService _menuService;
    private List<OrderDto>? _allOrders;
    private Dictionary<int, string> _stationNames = new();
    private Dictionary<int, string> _locationNames = new();
    private const int PageSize = 5;

    public OrderHistoryVM(IApiOrderService orderService, IApiMenuService menuService)
    {
        _orderService = orderService;
        _menuService = menuService;
    }

    public List<OrderDto>? Orders { get; private set; }
    public OrderDto? SelectedOrder { get; private set; }
    public int DisplayedOrderCount { get; private set; }
    public int TotalOrderCount => FilteredOrders?.Count ?? 0;
    public string? FilterType { get; set; }
    public bool IsLoading { get; private set; }
    public bool HasMoreOrders => DisplayedOrderCount < TotalOrderCount;

    private List<OrderDto>? FilteredOrders
    {
        get
        {
            if (_allOrders == null) return null;

            return FilterType switch
            {
                "card" => _allOrders.Where(o => IsCardPayment(o)).ToList(),
                "swipe" => _allOrders.Where(o => !IsCardPayment(o)).ToList(),
                _ => _allOrders
            };
        }
    }

    public async Task InitializeAsync()
    {
        IsLoading = true;

        try
        {
            _allOrders = await _orderService.GetOrdersByCustomerEmail();
            _allOrders = _allOrders.OrderByDescending(o => o.OrderTime).ToList();

            var stationIds = _allOrders
                .SelectMany(o => o.FoodItems)
                .Select(f => f.StationId)
                .Where(s => s.HasValue)
                .Select(s => s!.Value)
                .Distinct();

            foreach (var stationId in stationIds)
            {
                if (!_stationNames.ContainsKey(stationId))
                {
                    var station = await _menuService.GetStationById(stationId);
                    _stationNames[stationId] = station?.StationName ?? $"Station {stationId}";
                }
            }

            var locationIds = _allOrders
                .SelectMany(o => o.FoodItems)
                .Where(f => !f.StationId.HasValue && f.LocationId.HasValue)
                .Select(f => f.LocationId!.Value)
                .Distinct();

            foreach (var locationId in locationIds)
            {
                if (!_locationNames.ContainsKey(locationId))
                {
                    var location = await _menuService.GetLocationById(locationId);
                    _locationNames[locationId] = location?.LocationName ?? $"Location {locationId}";
                }
            }

            ApplyFilter();

            if (Orders?.Any() == true)
            {
                SelectedOrder = Orders.First();
            }
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void ApplyFilter()
    {
        var filtered = FilteredOrders ?? new List<OrderDto>();
        DisplayedOrderCount = Math.Min(PageSize, filtered.Count);
        Orders = filtered.Take(DisplayedOrderCount).ToList();
    }

    public void SelectOrder(OrderDto order)
    {
        SelectedOrder = order;
    }

    public void LoadMoreOrders()
    {
        var filtered = FilteredOrders ?? new List<OrderDto>();
        DisplayedOrderCount = Math.Min(DisplayedOrderCount + PageSize, filtered.Count);
        Orders = filtered.Take(DisplayedOrderCount).ToList();
    }

    public void SetFilter(string? filterType)
    {
        FilterType = filterType;
        ApplyFilter();

        if (SelectedOrder != null && Orders?.Contains(SelectedOrder) != true)
        {
            SelectedOrder = Orders?.FirstOrDefault();
        }
    }

    public bool IsCardPayment(OrderDto order)
    {
        return order.TotalPrice.HasValue && order.TotalPrice > 0;
    }

    public int GetItemCount(OrderDto order)
    {
        return order.FoodItems.Count;
    }

    public decimal GetSubtotal(OrderDto order)
    {
        if (!IsCardPayment(order)) return 0;

        return order.FoodItems.Sum(f => f.CardCost ?? 0);
    }

    public decimal GetTax(OrderDto order)
    {
        return order.Tax;
    }

    public decimal GetTotal(OrderDto order)
    {
        if (!IsCardPayment(order)) return 0;

        return (order.TotalPrice ?? 0);
    }

    public int GetSwipeCount(OrderDto order)
    {
        return order.TotalSwipe ?? order.FoodItems.Sum(f => f.SwipeCost ?? 0);
    }

    public string GetLocationLabel(int? stationId, int? locationId)
    {
        if (stationId.HasValue)
            return _stationNames.TryGetValue(stationId.Value, out var name) ? name : $"Station {stationId}";

        if (locationId.HasValue)
            return _locationNames.TryGetValue(locationId.Value, out var locName) ? locName : $"Location {locationId}";

        return string.Empty;
    }


}
