using System.Net.Http.Json;
using Cafeteria.Shared.DTOs.Menu;
using Cafeteria.Shared.DTOs.Order;
using Dapper;
using Npgsql;
using static Cafeteria.IntegrationTests.Api.SqlInsertQueries;

namespace Cafeteria.IntegrationTests.Api;

[Collection("Database")]
public class OrderIntegrationTests : IDisposable
{
    private readonly DatabaseFixture _fixture;
    private readonly HttpClient _client;
    private readonly NpgsqlConnection _connection;

    public OrderIntegrationTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
        _client = _fixture.Client;
        _connection = _fixture.GetConnection();
    }

    public void Dispose()
    {
        _connection?.Dispose();
    }

    [Fact]
    public async Task CreateOrder_AddsNewOrder()
    {
        var newOrder = new BrowserOrder
        {
            IsCardOrder = true,
            Location = new LocationDto { Id = 1, LocationName = "Test Location", LocationDescription = "Test Location Description" },
            StationId = 1,
            StationName = "Test Station",
            Entrees = new List<OrderEntreeItem>
            {
                new OrderEntreeItem
                {
                    Entree = new EntreeDto { Id = 1, EntreeName = "Test Entree", EntreePrice = 10.99m, StationId = 1 },
                    SelectedOptions = new List<SelectedFoodOption>
                    {
                        new SelectedFoodOption
                        {
                            Option = new FoodOptionDto { FoodOptionName = "Lettuce" },
                            OptionType = new FoodOptionTypeDto { Id = 1, FoodOptionTypeName = "Toppings", RequiredAmount = 0, IncludedAmount = 10, MaxAmount = 10, FoodOptionPrice = 0 }
                        },
                        new SelectedFoodOption
                        {
                            Option = new FoodOptionDto { FoodOptionName = "Tomato" },
                            OptionType = new FoodOptionTypeDto { Id = 1, FoodOptionTypeName = "Toppings", RequiredAmount = 0, IncludedAmount = 10, MaxAmount = 10, FoodOptionPrice = 0 }
                        }
                    }
                }
            },
            Sides = new List<OrderSideItem>
            {
                new OrderSideItem
                {
                    Side = new SideDto { Id = 1, SideName = "Test Side", SidePrice = 5.00m, StationId = 2 },
                    SelectedOptions = new List<SelectedFoodOption>()
                }
            },
            Drinks = new List<DrinkDto>()
        };

        var response = await _client.PostAsJsonAsync("/api/order", newOrder);
        response.EnsureSuccessStatusCode();
        var createdOrder = await response.Content.ReadFromJsonAsync<OrderDto>();

        Assert.NotNull(createdOrder);
        Assert.True(createdOrder.Id > 0);
        Assert.Equal(15.99m, createdOrder.TotalPrice);

        var persistedOrder = await _connection.QuerySingleAsync<OrderDto>(
            @"SELECT id AS Id,
                     order_time AS OrderTime,
                     total_price AS TotalPrice,
                     tax AS Tax,
                     total_swipe AS TotalSwipe
              FROM cafeteria.order
              WHERE id = @Id",
            new { createdOrder.Id }
        );

        Assert.Equal(createdOrder.Id, persistedOrder.Id);
        Assert.Equal(createdOrder.TotalPrice, persistedOrder.TotalPrice);
        Assert.Equal(createdOrder.Tax, persistedOrder.Tax);
        Assert.Equal(createdOrder.TotalSwipe, persistedOrder.TotalSwipe);

        Assert.Equal(2, createdOrder.FoodItems.Count);
        Assert.Equal(2, createdOrder.FoodItems[0].Options.Count);
        Assert.Equal("Lettuce", createdOrder.FoodItems[0].Options[0].FoodOptionName);
    }

    [Fact]
    public async Task GetOrderById_ReturnsCorrectOrder()
    {
        // Create a test order with food items and options
        var orderId = _connection.ExecuteScalar<int>(
            InsertOrderSql + " RETURNING id",
            new
            {
                CustomerBadgerId = 1000001,
                TotalPrice = 12.98m,
                Tax = 1.04m,
                TotalSwipe = 1
            }
        );

        var foodItem1Id = _connection.ExecuteScalar<int>(
            InsertFoodItemSql + " RETURNING id",
            new
            {
                Name = "Test Burger",
                OrderId = orderId,
                StationId = 1,
                SaleCardId = (int?)null,
                SaleSwipeId = (int?)null,
                SwipeCost = 0,
                CardCost = 8.99m,
                Special = false
            }
        );

        _connection.Execute(
            InsertFoodItemOptionSql,
            new[]
            {
                new { FoodItemId = foodItem1Id, FoodOptionName = "Lettuce" },
                new { FoodItemId = foodItem1Id, FoodOptionName = "Tomato" }
            }
        );

        var foodItem2Id = _connection.ExecuteScalar<int>(
            InsertFoodItemSql + " RETURNING id",
            new
            {
                Name = "French Fries",
                OrderId = orderId,
                StationId = 1,
                SaleCardId = (int?)null,
                SaleSwipeId = (int?)null,
                SwipeCost = 1,
                CardCost = 0m,
                Special = false
            }
        );

        var response = await _client.GetAsync($"/api/order/{orderId}");
        response.EnsureSuccessStatusCode();
        var order = await response.Content.ReadFromJsonAsync<OrderDto>();

        Assert.NotNull(order);
        Assert.Equal(orderId, order.Id);
        Assert.Equal(12.98m, order.TotalPrice);
        Assert.Equal(2, order.FoodItems.Count);
        Assert.Equal(2, order.FoodItems[0].Options.Count);
    }

    [Fact]
    public async Task GetOrderById_ReturnsNotFound_WhenOrderDoesNotExist()
    {
        var response = await _client.GetAsync("/api/order/99999");
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetAllOrders_ReturnsAllOrders()
    {
        // Create a couple of test orders
        var orderId1 = _connection.ExecuteScalar<int>(
            InsertOrderSql + " RETURNING id",
            new
            {
                CustomerBadgerId = 1000001,
                TotalPrice = 10.99m,
                Tax = 0.88m,
                TotalSwipe = 0
            }
        );

        var orderId2 = _connection.ExecuteScalar<int>(
            InsertOrderSql + " RETURNING id",
            new
            {
                CustomerBadgerId = 1000001,
                TotalPrice = 15.49m,
                Tax = 1.24m,
                TotalSwipe = 1
            }
        );

        var response = await _client.GetAsync("/api/order");
        response.EnsureSuccessStatusCode();
        var orders = await response.Content.ReadFromJsonAsync<List<OrderDto>>();

        Assert.NotNull(orders);
        Assert.True(orders.Count >= 2);
        Assert.Contains(orders, o => o.Id == orderId1);
        Assert.Contains(orders, o => o.Id == orderId2);
    }

    [Fact]
    public async Task GetAllOrdersWithCustomer_ReturnsOrdersWithCustomerInfo()
    {
        // Create a test order
        var orderId = _connection.ExecuteScalar<int>(
            InsertOrderSql + " RETURNING id",
            new
            {
                CustomerBadgerId = 1000001,
                TotalPrice = 12.99m,
                Tax = 1.04m,
                TotalSwipe = 0
            }
        );

        var response = await _client.GetAsync("/api/order/with-customer");
        response.EnsureSuccessStatusCode();
        var orders = await response.Content.ReadFromJsonAsync<List<OrderWithCustomerDto>>();

        Assert.NotNull(orders);
        Assert.True(orders.Count >= 1);
        // Verify that customer info is included
        Assert.All(orders, order => Assert.NotNull(order.CustomerName));
    }

    [Fact]
    public async Task GetOrderWithCustomerById_ReturnsOrderWithCustomerInfo()
    {
        // Create a test order
        var orderId = _connection.ExecuteScalar<int>(
            InsertOrderSql + " RETURNING id",
            new
            {
                CustomerBadgerId = 1000001,
                TotalPrice = 16.49m,
                Tax = 1.32m,
                TotalSwipe = 1
            }
        );

        var response = await _client.GetAsync($"/api/order/with-customer/{orderId}");
        response.EnsureSuccessStatusCode();
        var order = await response.Content.ReadFromJsonAsync<OrderWithCustomerDto>();

        Assert.NotNull(order);
        Assert.Equal(orderId, order.Id);
        Assert.NotNull(order.CustomerName);
        Assert.NotNull(order.CustomerEmail);
    }

    [Fact]
    public async Task GetOrderWithCustomerById_ReturnsNotFound_WhenOrderDoesNotExist()
    {
        var response = await _client.GetAsync("/api/order/with-customer/99999");
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetOrdersByCustomer_ReturnsOrdersForSpecificBadgerId()
    {
        // Use pre-loaded customer with badge ID 1001234
        var response = await _client.GetAsync("/api/order/customer/1001234");
        response.EnsureSuccessStatusCode();
        var orders = await response.Content.ReadFromJsonAsync<List<OrderWithCustomerDto>>();

        Assert.NotNull(orders);
        // Verify all orders belong to the requested customer
        Assert.All(orders, order => Assert.Equal(1001234, order.CustomerBadgerId));
    }

    [Fact]
    public async Task GetOrdersByCustomerEmail_ReturnsOrdersForAuthenticatedUser()
    {
        // The mock authentication handler provides email "test@example.com"
        // First ensure the customer exists
        var response = await _client.PostAsync("/api/customer/check", null);
        response.EnsureSuccessStatusCode();

        // Now retrieve orders for that customer
        var ordersResponse = await _client.GetAsync("/api/order/customer-email");
        ordersResponse.EnsureSuccessStatusCode();
        var orders = await ordersResponse.Content.ReadFromJsonAsync<List<OrderDto>>();

        Assert.NotNull(orders);
        // Should be empty or contain orders, but shouldn't error
    }
}
