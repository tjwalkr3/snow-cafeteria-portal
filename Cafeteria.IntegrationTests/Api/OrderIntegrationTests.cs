using System.Net.Http.Json;
using Cafeteria.Shared.DTOs;
using Npgsql;

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
        var newOrder = new CreateOrderDto
        {
            TotalPrice = 15.99m,
            Tax = 1.24m,
            TotalSwipe = 0,
            FoodItems = new List<CreateFoodItemDto>
            {
                new CreateFoodItemDto
                {
                    Name = "Test Entree",
                    StationId = 1,
                    CardCost = 10.99m,
                    Special = false,
                    Options = new List<CreateFoodItemOptionDto>
                    {
                        new CreateFoodItemOptionDto { FoodOptionName = "Lettuce" },
                        new CreateFoodItemOptionDto { FoodOptionName = "Tomato" }
                    }
                },
                new CreateFoodItemDto
                {
                    Name = "Test Side",
                    StationId = 2,
                    SwipeCost = 1,
                    Special = true,
                    Options = new List<CreateFoodItemOptionDto>()
                }
            }
        };

        var response = await _client.PostAsJsonAsync("/api/order", newOrder);
        response.EnsureSuccessStatusCode();
        var createdOrder = await response.Content.ReadFromJsonAsync<OrderDto>();

        Assert.NotNull(createdOrder);
        Assert.True(createdOrder.Id > 0);
        Assert.Equal(newOrder.TotalPrice, createdOrder.TotalPrice);
        Assert.Equal(2, createdOrder.FoodItems.Count);
        Assert.Equal(2, createdOrder.FoodItems[0].Options.Count);
        Assert.Equal("Lettuce", createdOrder.FoodItems[0].Options[0].FoodOptionName);
    }

    [Fact]
    public async Task GetOrderById_ReturnsCorrectOrder()
    {
        // Use pre-loaded order with ID 1
        var response = await _client.GetAsync("/api/order/1");
        response.EnsureSuccessStatusCode();
        var order = await response.Content.ReadFromJsonAsync<OrderDto>();

        Assert.NotNull(order);
        Assert.Equal(1, order.Id);
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
        var response = await _client.GetAsync("/api/order");
        response.EnsureSuccessStatusCode();
        var orders = await response.Content.ReadFromJsonAsync<List<OrderDto>>();

        Assert.NotNull(orders);
        Assert.True(orders.Count >= 2);
        Assert.Contains(orders, o => o.Id == 1);
        Assert.Contains(orders, o => o.Id == 2);
    }
}
