namespace Cafeteria.Shared;

public class OrderService
{
    private readonly List<Order> _orders = new();
    private int _nextOrderId = 1;

    public IReadOnlyList<Order> GetAllOrders()
    {
        return _orders.AsReadOnly();
    }

    public IReadOnlyList<Order> GetOrdersByCustomer(string customerName)
    {
        return _orders
            .Where(o => string.Equals(o.CustomerName, customerName, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(o => o.OrderDate)
            .ToList()
            .AsReadOnly();
    }

    public Order? GetOrderById(int orderId)
    {
        return _orders.FirstOrDefault(o => o.Id == orderId);
    }

    public Order CreateOrder(string customerName, string? customerEmail, IList<OrderItem> items, string? notes = null)
    {
        if (string.IsNullOrWhiteSpace(customerName))
            throw new ArgumentException("Customer name is required", nameof(customerName));

        if (items == null || !items.Any())
            throw new ArgumentException("Order must contain at least one item", nameof(items));

        var totalAmount = items.Sum(item => item.TotalPrice);

        var order = new Order(
            id: _nextOrderId++,
            customerName: customerName,
            orderDate: DateTime.Now,
            status: OrderStatus.Pending,
            items: items.ToList().AsReadOnly(),
            totalAmount: totalAmount,
            customerEmail: customerEmail,
            notes: notes
        );

        _orders.Add(order);
        return order;
    }

    public Order UpdateOrderStatus(int orderId, OrderStatus newStatus)
    {
        var order = GetOrderById(orderId);
        if (order == null)
            throw new ArgumentException($"Order with ID {orderId} not found", nameof(orderId));

        var updatedOrder = new Order(
            order.Id,
            order.CustomerName,
            order.OrderDate,
            newStatus,
            order.Items,
            order.TotalAmount,
            order.CustomerEmail,
            order.Notes
        );

        var index = _orders.FindIndex(o => o.Id == orderId);
        _orders[index] = updatedOrder;

        return updatedOrder;
    }

    public void CancelOrder(int orderId)
    {
        UpdateOrderStatus(orderId, OrderStatus.Cancelled);
    }

    public IReadOnlyList<Order> GetSampleOrders()
    {
        if (_orders.Any()) return GetAllOrders();

        // Create sample orders for demonstration
        var builder = new FoodItemBuilderService();
        var sampleItems = new List<FoodItem>
        {
            builder.Reset().SetName("Classic Pizza").SetImageUrl("https://via.placeholder.com/300x200?text=Pizza").SetPrice(12.99m).Build(),
            builder.Reset().SetName("Caesar Salad").SetImageUrl("https://via.placeholder.com/300x200?text=Salad").SetPrice(8.50m).Build(),
            builder.Reset().SetName("Cheeseburger").SetImageUrl("https://via.placeholder.com/300x200?text=Burger").SetPrice(10.75m).Build()
        };

        // Sample Order 1
        CreateOrder("John Doe", "john@example.com", new List<OrderItem>
        {
            new OrderItem(sampleItems[0], 2, 12.99m),
            new OrderItem(sampleItems[1], 1, 8.50m)
        }, "Extra cheese on pizza");

        // Sample Order 2
        CreateOrder("Jane Smith", "jane@example.com", new List<OrderItem>
        {
            new OrderItem(sampleItems[2], 1, 10.75m)
        });

        // Sample Order 3
        CreateOrder("Mike Johnson", null, new List<OrderItem>
        {
            new OrderItem(sampleItems[0], 1, 12.99m),
            new OrderItem(sampleItems[2], 2, 10.75m)
        }, "No onions on burger");

        // Update some order statuses for variety
        UpdateOrderStatus(1, OrderStatus.Ready);
        UpdateOrderStatus(2, OrderStatus.Preparing);

        return GetAllOrders();
    }
}