using Cafeteria.Shared;

namespace Cafeteria.Api.Tests;

public class OrderServiceTests
{
    [Fact]
    public void CreateOrder_WithValidData_CreatesOrder()
    {
        var orderService = new OrderService();
        var foodItem = CreateSampleFoodItem();
        var items = new List<OrderItem> { new OrderItem(foodItem, 2, 10.00m) };

        var order = orderService.CreateOrder("John Doe", "john@test.com", items);

        Assert.Equal("John Doe", order.CustomerName);
        Assert.Equal("john@test.com", order.CustomerEmail);
        Assert.Equal(OrderStatus.Pending, order.Status);
        Assert.Equal(20.00m, order.TotalAmount);
        Assert.Single(order.Items);
    }

    [Fact]
    public void CreateOrder_WithEmptyName_ThrowsArgumentException()
    {
        var orderService = new OrderService();
        var items = new List<OrderItem> { new OrderItem(CreateSampleFoodItem(), 1, 10.00m) };

        var exception = Assert.Throws<ArgumentException>(() =>
            orderService.CreateOrder("", null, items));

        Assert.Equal("Customer name is required (Parameter 'customerName')", exception.Message);
    }

    [Fact]
    public void UpdateOrderStatus_WithValidId_UpdatesStatus()
    {
        var orderService = new OrderService();
        var items = new List<OrderItem> { new OrderItem(CreateSampleFoodItem(), 1, 10.00m) };
        var order = orderService.CreateOrder("Jane Smith", null, items);

        var updatedOrder = orderService.UpdateOrderStatus(order.Id, OrderStatus.Ready);

        Assert.Equal(OrderStatus.Ready, updatedOrder.Status);
        Assert.Equal(order.Id, updatedOrder.Id);
    }

    [Fact]
    public void GetOrdersByCustomer_ReturnsCustomerOrders()
    {
        var orderService = new OrderService();
        var items = new List<OrderItem> { new OrderItem(CreateSampleFoodItem(), 1, 10.00m) };

        orderService.CreateOrder("John Doe", null, items);
        orderService.CreateOrder("Jane Smith", null, items);
        orderService.CreateOrder("John Doe", null, items);

        var johnOrders = orderService.GetOrdersByCustomer("John Doe");

        Assert.Equal(2, johnOrders.Count);
        Assert.All(johnOrders, order => Assert.Equal("John Doe", order.CustomerName));
    }

    [Fact]
    public void OrderItem_CalculatesTotalPrice_Correctly()
    {
        var foodItem = CreateSampleFoodItem();
        var orderItem = new OrderItem(foodItem, 3, 5.50m);

        Assert.Equal(16.50m, orderItem.TotalPrice);
        Assert.Equal(3, orderItem.Quantity);
        Assert.Equal(5.50m, orderItem.UnitPrice);
    }

    private FoodItem CreateSampleFoodItem()
    {
        var builder = new FoodItemBuilderService();
        return builder.SetName("Test Item").SetImageUrl("test.jpg").SetPrice(10.00m).Build();
    }
}