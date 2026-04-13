using Cafeteria.Shared.DTOs.Order;

namespace Cafeteria.Customer.Components.Pages.OrderHistory;

public interface IOrderHistoryVM
{
    List<OrderDto>? Orders { get; }
    OrderDto? SelectedOrder { get; }
    int DisplayedOrderCount { get; }
    int TotalOrderCount { get; }
    string? FilterType { get; set; }
    bool IsLoading { get; }
    bool HasMoreOrders { get; }

    Task InitializeAsync();
    void SelectOrder(OrderDto order);
    OrderDto? FindOrderById(int id);
    void LoadMoreOrders();
    void SetFilter(string? filterType);
    bool IsCardPayment(OrderDto order);
    int GetItemCount(OrderDto order);
    decimal GetSubtotal(OrderDto order);
    decimal GetTax(OrderDto order);
    decimal GetTotal(OrderDto order);
    int GetSwipeCount(OrderDto order);
    List<(FoodItemDto Item, int Count)> GetGroupedFoodItems(OrderDto order);
    string GetLocationLabel(int? stationId, int? locationId);
}
